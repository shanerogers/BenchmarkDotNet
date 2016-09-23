﻿using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Horology;

namespace BenchmarkDotNet.Jobs
{
    public sealed class RunMode
    {
        public static readonly RunMode Default = new RunMode();

        public static readonly JobMutator Dry = CreateMutator(nameof(Dry), 1, 1, 1, Engines.RunStrategy.ColdStart).Add(Default.UnrollFactor.Mutate(1));
        public static readonly JobMutator Short = CreateMutator(nameof(Short), 1, 3, 3);
        public static readonly JobMutator Medium = CreateMutator(nameof(Medium), 2, 10, 15);
        public static readonly JobMutator Long = CreateMutator(nameof(Long), 3, 15, 100);
        public static readonly JobMutator VeryLong = CreateMutator(nameof(VeryLong), 4, 30, 500);

        private static ICharacteristic<T> Create<T>(string id) => Characteristic<T>.Create("Run", id);

        /// <summary>
        /// Available values: Throughput and ColdStart.
        ///     Throughput: default strategy which allows to get good precision level.
        ///     ColdStart: should be used only for measuring cold start of the application or testing purpose.
        /// </summary>
        public ICharacteristic<RunStrategy> RunStrategy { get; private set; } = Create<RunStrategy>(nameof(RunStrategy));

        /// <summary>
        /// How many times we should launch process with target benchmark.
        /// </summary>
        public ICharacteristic<int> LaunchCount { get; private set; } = Create<int>(nameof(LaunchCount));

        /// <summary>
        /// How many warmup iterations should be performed.
        /// </summary>
        public ICharacteristic<int> WarmupCount { get; private set; } = Create<int>(nameof(WarmupCount));

        /// <summary>
        /// How many target iterations should be performed
        /// </summary>
        public ICharacteristic<int> TargetCount { get; private set; } = Create<int>(nameof(TargetCount));

        /// <summary>
        /// Desired time of execution of an iteration.
        /// </summary>
        public ICharacteristic<TimeInterval> IterationTime { get; private set; } = Create<TimeInterval>(nameof(IterationTime));

        /// <summary>
        /// Invocation count in a single iteration.
        /// If specified, <see cref="IterationTime"/> will be ignored.
        /// If specified, it must be a multiple of <see cref="UnrollFactor"/>.
        /// </summary>
        public ICharacteristic<int> InvocationCount { get; private set; } = Create<int>(nameof(InvocationCount));

        /// <summary>
        /// How many times the benchmark method will be invoked per one iteration of a generated loop.
        /// </summary>
        public ICharacteristic<int> UnrollFactor { get; private set; }= Create<int>(nameof(UnrollFactor));

        public static JobMutator CreateMutator(string id, int launchCount, int warmupCount, int targetCount,
            RunStrategy strategy = Engines.RunStrategy.Throughput)
        {
            return new JobMutator(id).Add(new CharacteristicSet(
                Default.LaunchCount.Mutate(launchCount),
                Default.WarmupCount.Mutate(warmupCount),
                Default.TargetCount.Mutate(targetCount),
                Default.RunStrategy.Mutate(strategy)
            ));
        }

        public static RunMode Parse(CharacteristicSet set)
        {
            var mode = new RunMode();
            mode.RunStrategy = mode.RunStrategy.Mutate(set);
            mode.LaunchCount = mode.LaunchCount.Mutate(set);
            mode.WarmupCount = mode.WarmupCount.Mutate(set);
            mode.TargetCount = mode.TargetCount.Mutate(set);
            mode.IterationTime = mode.IterationTime.Mutate(set);
            mode.InvocationCount = mode.InvocationCount.Mutate(set);
            mode.UnrollFactor = mode.UnrollFactor.Mutate(set);
            return mode;
        }

        public CharacteristicSet ToSet() => new CharacteristicSet(
            RunStrategy,
            LaunchCount,
            WarmupCount,
            TargetCount,
            IterationTime,
            InvocationCount,
            UnrollFactor
        );
    }
}