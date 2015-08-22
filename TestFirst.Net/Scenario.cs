using System;
using System.Collections.Generic;
using System.Linq;
using TestFirst.Net.Util;

namespace TestFirst.Net
{
    /// <summary>
    /// The starting point to test a scenario. Have to start with a a call to one of the 'Given(....)' steps, and end with at least one
    /// of the 'Then(...)' steps for the scenario to pass.
    /// </summary>
    public class Scenario:IDisposable
    {
        private static readonly IStepArgDependencyInjector NullInjector = new NullStepArgDependencyInjector();

        private State m_state = State.NotRun;
        private Exception m_failingException;
        private readonly IDictionary<String,bool?> m_stepLog = new Dictionary<string, bool?>();
        private readonly IList<IRunOnScenarioEnd> m_onScenarioEndListeners = new List<IRunOnScenarioEnd>(); 
                
        private enum State
        {
            NotRun,Passed,Failed
        }

        /// <summary>
        /// Human readable label for this scenario. Optional
        /// </summary>
        public String Title { private set; get; }

        /// <summary>
        /// Injector used to inject test dependencies
        /// </summary>
        private readonly IStepArgDependencyInjector m_stepArgDependencyInjector;

        public Scenario()
            : this(null)
        {
        }

        public Scenario(string title):this(title, null)
        {
        }

        public Scenario(string title, IStepArgDependencyInjector stepDependencyInjector)
        {
            Title = title ?? "<No Name>";
            m_stepArgDependencyInjector = stepDependencyInjector??NullInjector;
        }

        /// <summary>
        /// Inject all dependencies into the given instance. This will call the underlying <see cref="IStepArgDependencyInjector"/>
        /// 
        /// Normally there should be no need to call this as all the various Given, When, Then methods will pass your
        /// args to this method anyway.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">the instance to have dependencies set on. Can be null or primitive in which case nothing is done</param>
        public void InjectDependencies<T>(T instance)
        {
            if (instance == null || instance.GetType().IsPrimitive)
            {
                return;
            }
            var onEndListener = instance as IRunOnScenarioEnd;
            if (onEndListener != null)
            {
                AddListener(onEndListener);
            }
            m_stepArgDependencyInjector.InjectDependencies(instance);
        }

        private Scenario AddListener(IRunOnScenarioEnd listener)
        {
            if (!m_onScenarioEndListeners.Contains(listener))
            {
                m_onScenarioEndListeners.Add(listener);
            }
            return this;
        }

        /// <summary>
        /// Syntactic sugar which does nothing. Allows starting off the steps where no givens are required
        /// </summary>
        /// <returns>the next step</returns>
        public GivenStep GivenNothing()
        {
            Step("GivenNothing()");
            return new GivenStep(this);
        }

        /// <summary>
        /// Invoke an inserter
        /// </summary>
        /// <returns>the next step</returns>
        public GivenStep Given(IInserter inserter)
        {
            return Givens.Given(this, inserter);
        }

        /// <summary>
        /// Invoke an updater
        /// </summary>
        /// <returns>the next step</returns>
        public GivenStep Given(IUpdater updater)
        {
            return Givens.Given(this, updater);
        }

        /// <summary>
        /// Invoke the given invoker
        /// </summary>
        /// <returns>the next step</returns>
        public GivenStep Given(IInvokable invokable)
        {
            return Givens.Given(this, invokable);
        }

        /// <summary>
        /// Warning method to catch the use of a matcher on a given. Always throws exception
        /// </summary>
        [Obsolete("This is not the method you want. You probably want an inserter instead. Keep this to warn of api incorrect usage")]
        public void Given(IMatcher notWhatYouWantUseInserterInstead)
        {
            throw Givens.NewGivenMatcherError();
        }

        /// <summary>
        /// Pass in an instance which is dependency injected
        /// </summary>
        /// <param name="instance">the instance to pass back</param>
        /// <returns>the next step</returns>
        public GivenStep Given(Object instance)
        {
            return Givens.Given(this, instance);
        }

        /// <summary>
        /// Invoke an action
        /// </summary>
        /// <returns>the next step</returns>
        public GivenStep Given(Action action)
        {
            return Givens.Given(this, action);
        }

        /// <summary>
        /// Invoke the given action passing in the current scenario
        /// </summary>
        /// <param name="scenarioExtensionAction"></param>
        /// <returns></returns>
        public GivenStep Given(Action<Scenario> scenarioExtensionAction)
        {
            return Givens.Given(this, scenarioExtensionAction);
        }

        /// <summary>
        /// Dispose the injector. Be sure to call <see cref="AssertHasRunAndPassed"/> beforehand
        /// </summary>
        public void Dispose()
        {
            m_stepArgDependencyInjector.Dispose();
        }

        /// <summary>
        /// Check that this scenario has had at least one 'Then' step invoked, and that all 'Thens' have been successful.
        /// 
        /// Throw an AssertionFailedException if these conditions do not hold true.
        /// 
        /// If this method does not throw an exception consider the scenario as passing
        /// </summary>
        public void AssertHasRunAndPassed()
        {
            RunOnEndListeners();

            if (m_state == State.NotRun)
            {
                TestFirstAssert.Fail("Scenario '{0}' has not run. Did you invoke Then(..)? (if previous errors this may have caused the no run). \n\n{1}", Title, NewFailedMsg());
            }
            
            if (m_state != State.Passed)
            {
                if (m_failingException != null)
                {
                    TestFirstAssert.Fail(m_failingException, "Scenario '{0}' failed with exception", Title);
                }
                else
                {
                    TestFirstAssert.Fail("Scenario '{0}' failed", Title);
                }
            }
        }

        private void RunOnEndListeners()
        {
            //run the scenario listeners invoking them in the order they were added
            if (m_onScenarioEndListeners != null && m_onScenarioEndListeners.Count > 0)
            {
                var listeners = new List<IRunOnScenarioEnd>(m_onScenarioEndListeners);
                listeners.Reverse();
                foreach (var listener in listeners)
                {
                    if (listener == null)
                    {
                        continue;
                    }
                    try
                    {
                        //don't catch exceptions, let them bubble up so we get a nice stack trace
                        listener.OnScenarioEnd();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Listener " + listener.GetType().FullName + " threw exception", e);
                    }
                }
            }
        }

        /// <summary>
        /// Notify the scenario a step has been invoked. This is used when generating error messages to show what has already been invoked
        /// </summary>
        private void 
        Step(string stepName, params object[] args)
        {
            int stepNum = m_stepLog.Count + 1;
            if (args != null)
            {
                stepName = String.Format(stepName, args);
            }
            var msg = "Step " + stepNum + " = " + stepName;
            //Console.WriteLine(msg);
            m_stepLog.Add(msg,null);
        }

        private void NewWhenStep()
        {
            m_state = State.NotRun;
        }

        /// <summary>
        /// Invoke the given action, setting the scenarios to failed if the action throws an exception. The exception 
        /// will be captured and then re-thrown
        /// </summary>
        private void InvokeOrFail(Action action)
        {
            try
            {
                if (action == null)
                {
                    throw new NullReferenceException("The passed in action was null. Can't invoke a null action");
                }
                action.Invoke();
                MarkLastStepPassed(true);
            }
            catch (Exception e)
            {
                MarkLastStepPassed(false);
                throw FailedWithError(e);
            }
        }

        private void InvokeOrFail(Action<Scenario> action)
        {
            try
            {
                if (action == null)
                {
                    throw new NullReferenceException("The passed in action was null. Can't invoke a null action");
                }
                action.Invoke(this);
                MarkLastStepPassed(true);
            }
            catch (Exception e)
            {
                MarkLastStepPassed(false);
                throw FailedWithError(e);
            }
        }

        /// <summary>
        /// Invoke the given function returning the result. Mark the scenario as failed if the function throws an exception
        /// </summary>
        private T InvokeOrFailReturningResult<T>(Func<T> function)
        {
            try
            {
                if (function == null)
                {
                    throw new NullReferenceException("The passed in function was null. Can't invoke a null function of Function<" + typeof(T).FullName + ">");
                }
                
                var t = function.Invoke();
                MarkLastStepPassed(true);
                return t;
            }
            catch (Exception e)
            {
                MarkLastStepPassed(false);
                throw FailedWithError(e);
            }
        }

        /// <summary>
        /// If passed, then mark scenario as having been run. Should only be called from 'Thens'
        /// </summary>
        private void ThenAssertMatchPassed<T>(Boolean passed, T actual, IMatcher<T> matcher,
                                                  IMatchDiagnostics diagnostics)
        {
            Passed(passed);
            FailIfNotPassed(passed,actual,matcher,diagnostics);
        }

        /// <summary>
        /// Mark the scenario as failed if the match failed. If it passed _dont_ mark scenario as having been run
        /// as this could be called from any step( given, check, when...)
        /// </summary>
        private void FailIfNotPassed<T>(Boolean passed, T actual, IMatcher<T> matcher,
                                          IMatchDiagnostics diagnostics)
        {
            MarkLastStepPassed(passed);
            if (!passed)
            {
                Passed(false);

                var desc = new Description();
                desc.Child("Steps were",  StepsToString());
                desc.Child("expected", matcher);
                if (actual is String) {
                    desc.Child ("but was", "'" + actual + "'");
                } else {
                    desc.Child ("but was", actual);
                }
                desc.Text("==== Diagnostics ====");
                desc.Child(diagnostics);

                TestFirstAssert.Fail(Environment.NewLine + desc);
            }
        }

        private void Passed(bool passed)
        {
            if (m_state == State.NotRun)
            {
                m_state = passed ? State.Passed : State.Failed;
            }
            else //only allow transition to failure. This allows for multiple 'Then(...)'
            {
                if (!passed)
                {
                    m_state = State.Failed;
                }
            }
            MarkLastStepPassed(passed);
        }

        //by marking a step successful we can determine whether a step failed after it
        // was invoked or while th ecaller was setting up their arguments to the step method
        private void MarkLastStepPassed(bool passed)
        {
            m_stepLog[m_stepLog.Last().Key] = passed;
        }

        private TestFirstException FailedWithError(Exception e)
        {
            m_state = State.Failed;
            m_failingException = e;
            return new TestFirstException(NewFailedMsg(),e);
        }

        private String NewFailedMsg()
        {
            var desc = new Description();
            desc.Child("Steps were", StepsToString());
           
            return desc.ToString();
        }

        /// <summary>
        /// Print the steps runs and teh current failing step
        /// </summary>
        /// <returns></returns>
        public String StepsToString()
        {
            var allStepsMarkedPassed = m_stepLog.All(p => p.Value.HasValue && p.Value.Value);
            var steps = m_stepLog.Select(entry => entry.Key);
            if (allStepsMarkedPassed) //must be error while setting up args to step in callers code
            {
                return String.Join(Environment.NewLine, steps) + Environment.NewLine + "Step " + (m_stepLog.Count+1) + " <== failed! (before step called)";    
            }            
            return String.Join(Environment.NewLine, steps) + " <== failed!";
        }     

        /// <summary>
        /// A step which does not hold any results from the previous operation
        /// </summary>
        public class GivenStep : WhenStep
        {
            internal GivenStep(Scenario scenario)
                : base(scenario)
            {
            }
            
            /// <summary>
            /// Warning method to catch the use of a matcher on a given. Always throws exception
            /// </summary>
            [Obsolete("This is not the method you want. You probably want an inserter instead. Keep this to warn of api incorrect usage")]
            public void Given(IMatcher notWhatYouWantUseInserterInstead)
            {
                throw Givens.NewGivenMatcherError();
            }
            /// <summary>
            /// Invoke an inserter
            /// </summary>
            /// <returns>the next step</returns>
            public GivenStep Given(IInserter inserter)
            {
                return Givens.Given(Scenario, inserter);
            }

            /// <summary>
            /// Invoke an updater
            /// </summary>
            /// <returns>the next step</returns>
            public GivenStep Given(IUpdater updater)
            {
                return Givens.Given(Scenario, updater);
            }

            /// <summary>
            /// Invoke the given action passing in the current scenario
            /// </summary>
            /// <param name="scenarioExtensionAction"></param>
            /// <returns></returns>
            public GivenStep Given(Action<Scenario> scenarioExtensionAction)
            {
                return Givens.Given(Scenario, scenarioExtensionAction);
            }

            /// <summary>
            /// Pass in an instance which can be passed to the nest step. 
            /// </summary>
            /// <param name="instance">the instance to pass back</param>
            /// <returns>a step containing the passed in instance</returns>
            public GivenStep Given(Object instance)
            {
                return Givens.Given(Scenario, instance);
            }

            /// <summary>
            /// Invoke an action
            /// </summary>
            /// <returns>the next step</returns>
            public GivenStep Given(Action givenAction)
            {
                return Givens.Given(Scenario, givenAction);
            }

            public GivenStep Given(Action<GivenStep> stepExtensionAction)
            {
                stepExtensionAction.Invoke(this);
                return this;
            }

            /// <summary>
            /// Invoke the given invoker
            /// </summary>
            /// <returns>the next step</returns>
            public GivenStep Given(IInvokable invokable)
            {
                return Givens.Given(Scenario, invokable);
            }

            /// <summary>
            /// Check that the current state is what is expected before carrying on. Useful when steps are failing. Fails the scenario
            /// if the matcher fails. Think of this as a sanity test of your test setup code
            /// </summary>
            /// <param name="fetcher">retreives the value to check</param>
            /// <param name="matcher">the matcher to check the value</param>
            /// <returns>the next step</returns>
            public GivenStep CheckingAssumption<T>(IFetcher<T> fetcher, IMatcher<T> matcher)
            {
                return Givens.Checking(Scenario, fetcher, matcher);
            }

            /// <summary>
            /// Check that the current state is what is expected before carrying on. Useful when steps are failing. Fails the scenario
            /// if the matcher fails. Think of this as a sanity test of your test setup code
            /// </summary>
            /// <param name="actual">the value to check</param>
            /// <param name="matcher">the matcher to check the value</param>
            /// <returns>the next step</returns>
            public GivenStep CheckingAssumption<T>(T actual, IMatcher<T> matcher)
            {
                return Givens.Checking(Scenario, actual, matcher);
            }

            /// <summary>
            /// Check that the current state is what is expected before carrying. Useful when steps are failing. Fails the scenario
            /// if the matcher fails. Think of this as a sanity test of your test setup code
            /// </summary>
            /// <param name="func">retreives the value to check</param>
            /// <param name="matcher">the matcher to check the value</param>
            /// <returns>the next step</returns>
            public GivenStep CheckingAssumption<T>(Func<T> func, IMatcher<T> matcher)
            {
                return Givens.Checking(Scenario, func, matcher);
            }

            /// <summary>
            /// Syntactic sugar which does nothing to allow explicitly skipping any 'when' steps
            /// </summary>
            /// <returns>the next step</returns>
            public WhenStep WhenNothing()
            {
                Scenario.NewWhenStep();
                Scenario.Step("WhenNothing()");
                return new WhenStep(Scenario);
            }
        }

        /// <summary>
        /// A step which does not hold any results from the previous operation
        /// </summary>
        public class WhenStep : ThenStep
        {
            internal WhenStep(Scenario scenario)
                : base(scenario)
            {
            }
        }

        /// <summary>
        /// A step which does not hold any results from the previous operation
        /// </summary>
        public class ThenStep
        {
            protected Scenario Scenario { get; private set; }

            /// <summary>
            /// Pass the given instance to the next step.
            /// </summary>
            /// <param name="actual">the instance to pass to the nest step</param>
            /// <returns>the next step containing the given instance</returns>
            public WhenStep When(Object actual)
            {
                return Whens.When(Scenario, actual);
            }

            public WhenStep When(IInvokable invokable)
            {
                return Whens.When(Scenario, invokable);
            }

            /// <summary>
            /// Invoke the given action, failing the scenario if it throws an exception
            /// </summary>
            /// <param name="action">the action to invoke</param>
            /// <returns>the next step</returns>
            public WhenStep When(Action action)
            {
                return Whens.When(Scenario, action);
            }

            /// <summary>
            /// Invoke the given function, failing the scenario if it throws an exception
            /// </summary>
            /// <returns>the next step</returns>
            public WhenStep When<T>(Func<T> function)
            {
                return Whens.When(Scenario, function);
            }

            internal ThenStep(Scenario scenario)
            {
                Scenario = scenario;
            }

            /// <summary>
            /// Invoke the given action, failing the scenario if it throws an exception
            /// </summary>
            /// <param name="action">the action to invoke</param>
            /// <returns>the next step</returns>
            public ThenStep Then(Action action)
            {
                return Thens.Then(Scenario, action);
            }

            /// <summary>
            /// Invoke the given fetcher, using the provided matcher to fail the scenario if it does not match the fetch result
            /// </summary>
            /// <param name="fetcher">the fetcher to invoke</param>
            /// <param name="matcher">the matcher to use</param>
            /// <returns>the next step</returns>
            public ThenStep Then<T>(IFetcher<T> fetcher, IMatcher<T> matcher)
            {
                return Thens.Then(Scenario, fetcher, matcher);
            }

            /// <summary>
            /// Using the provided matcher to fail the scenario if it does not match the given actual value
            /// </summary>
            /// <param name="actual">the value to check</param>
            /// <param name="matcher">the matcher to use</param>
            /// <returns>the next step</returns>
            public ThenStep Then<T>(T actual, IMatcher<T> matcher)
            {
                return Thens.Then(Scenario, actual, matcher);
            }

            /// <summary>
            /// Invoke the given function, using the provided matcher to fail the scenario if it does not match the function output
            /// </summary>
            /// <param name="func">the function to invoke</param>
            /// <param name="matcher">the matcher to use</param>
            /// <returns>the next step containing the result of the function</returns>
            public ThenStep Then<T>(Func<T> func, IMatcher<T> matcher)
            {
                return Thens.Then(Scenario, func, matcher);
            }
        }

        private static class Givens
        {

            internal static NotImplementedException NewGivenMatcherError()
            {
                return new NotImplementedException("You passed a matcher to a Given which is not likely your intent, instead you probably wanted to pass in an inserter or action instead");      
            }
            internal static GivenStep Given(Scenario scenario, Action action)
            {
                scenario.Step("Given(Action)");
                scenario.InjectDependencies(action);
                scenario.InvokeOrFail(action);
                return new GivenStep(scenario);
            }

            internal static GivenStep Given(Scenario scenario, IInserter inserter)
            {
                var rootInserter = InserterUtil.GetRootInserter(inserter);
                if (!ReferenceEquals(rootInserter, inserter))
                {
                    scenario.Step("Given(IInserter({0})) rootInserter -to-> IInserter({1})", inserter.GetType().Name, rootInserter.GetType().Name);
                }
                else
                {
                    scenario.Step("Given(IInserter({0}))", rootInserter.GetType().Name);
                }
                scenario.InjectDependencies(rootInserter);

                scenario.InvokeOrFail(rootInserter.Insert);
                return new GivenStep(scenario);
            }

  
            internal static GivenStep Given(Scenario scenario, IUpdater inserter)
            {
                scenario.Step("Given(IUpdater({0}))", inserter.GetType().Name);
                scenario.InjectDependencies(inserter);

                scenario.InvokeOrFail(inserter.Update);
                return new GivenStep(scenario);
            }

            internal static GivenStep Given(Scenario scenario, Action<Scenario> action)
            {
                scenario.Step("Given(Action<Scenario>) (Scenario extension action)");
                scenario.InjectDependencies(action);
                scenario.InvokeOrFail(action);
                return new GivenStep(scenario);
            }

            internal static GivenStep Given(Scenario scenario, IInvokable invokable)
            {
                scenario.Step("Given({0})", invokable == null ? "Invokable" : invokable.GetType().Name);
                if (invokable == null)
                {
                    scenario.MarkLastStepPassed(false);
                    throw scenario.FailedWithError(new NullReferenceException("Invokable is null"));
                }
                scenario.InjectDependencies(invokable);

                scenario.InvokeOrFail(invokable.Invoke);
                return new GivenStep(scenario);
            }

            internal static GivenStep Given(Scenario scenario, Object instance)
            {
                scenario.Step("Given({0})",instance==null?"Object":instance.GetType().Name);
                scenario.InjectDependencies(instance);

                scenario.MarkLastStepPassed(true);
                return new GivenStep(scenario);
            }

            internal static GivenStep Checking<T>(Scenario scenario, IFetcher<T> fetcher, IMatcher<T> matcher)
            {
                scenario.Step("CheckingAssumption(IFetcher<{0}>,IMatcher<{0}>({1}))", typeof(T).Name, matcher.GetType().Name);
                scenario.InjectDependencies(fetcher);
                scenario.InjectDependencies(matcher);

                T result = scenario.InvokeOrFailReturningResult(fetcher.Fetch);
                return InternalMatch(scenario, result, matcher);
            }

            internal static GivenStep Checking<T>(Scenario scenario, Func<T> func, IMatcher<T> matcher)
            {
                scenario.Step("CheckingAssumption(Func<{0}>,IMatcher<{0}>({1}))", typeof(T).Name, matcher.GetType().Name);
                scenario.InjectDependencies(func);
                scenario.InjectDependencies(matcher);

                T result = scenario.InvokeOrFailReturningResult(func);
                return InternalMatch(scenario, result, matcher);
            }


            internal static GivenStep Checking<T>(Scenario scenario, T result, IMatcher<T> matcher)
            {
                scenario.Step("CheckingAssumption({0},IMatcher<{0}>({1}))", typeof(T).Name, matcher.GetType().Name);
                scenario.InjectDependencies(result);
                scenario.InjectDependencies(matcher);

                return InternalMatch(scenario, result, matcher);
            }

            private static GivenStep InternalMatch<T>(Scenario scenario, T result, IMatcher<T> matcher)
            {
                bool passed;
                var diagnostics = new MatchDiagnostics();
                try
                {
                    passed = matcher.Matches(result, diagnostics);
                }
                catch (Exception e)
                {
                    throw scenario.FailedWithError(e);
                }                
                scenario.FailIfNotPassed(passed, result, matcher, diagnostics);

                return new GivenStep(scenario);
            }
        }

        private static class Whens
        {
            internal static WhenStep When(Scenario scenario, Object result)
            {
                scenario.NewWhenStep();
                scenario.Step("When({0})", result == null ? "Object:null" : result.GetType().Name);
                scenario.InjectDependencies(result);

                scenario.MarkLastStepPassed(true);
                return new WhenStep(scenario);
            }

            internal static WhenStep When(Scenario scenario, IInvokable invokable)
            {
                scenario.NewWhenStep();
                scenario.Step("When({0})", invokable.GetType().Name);
                scenario.InjectDependencies(invokable);

                scenario.InvokeOrFail(invokable.Invoke);
                return new WhenStep(scenario);
            }

            internal static WhenStep When(Scenario scenario, Action action)
            {
                scenario.NewWhenStep();
                scenario.Step("When(Action)");
                scenario.InjectDependencies(action);
                scenario.InvokeOrFail(action.Invoke);
                return new WhenStep(scenario);
            }

            internal static WhenStep When<T>(Scenario scenario, Func<T> function)
            {
                scenario.NewWhenStep();
                scenario.Step("When(Func<{0}>)", typeof(T).Name);
                scenario.InjectDependencies(function);
                scenario.InvokeOrFailReturningResult(function.Invoke);
                return new WhenStep(scenario);
            }
        }

        private static class Thens
        {
            internal static ThenStep Then<T>(Scenario scenario, IFetcher<T> fetcher, IMatcher<T> matcher)
            {
                scenario.Step("Then(IFetcher<{0}>,IMatcher<{0}>({1}))", typeof(T).Name, matcher.GetType().Name);
                scenario.InjectDependencies(fetcher);
                scenario.InjectDependencies(matcher);

                T result = scenario.InvokeOrFailReturningResult(fetcher.Fetch);
                return InternalMatch(scenario, result, matcher);
            }
            
            
            internal static ThenStep Then<T>(Scenario scenario, Func<T> func, IMatcher<T> matcher)
            {
                scenario.Step("Then(Func<{0}>,IMatcher<{0}>({1}))", typeof(T).Name, matcher.GetType().Name);
                scenario.InjectDependencies(func);
                scenario.InjectDependencies(matcher);

                T result = scenario.InvokeOrFailReturningResult(func);
                return InternalMatch(scenario, result, matcher);
            }

            internal static ThenStep Then<T>(Scenario scenario, T matcherArg, IMatcher<T> matcher)
            {
                scenario.Step("Then({0},IMatcher<{0}>({1}))", typeof(T).Name, matcher.GetType().Name);
                scenario.InjectDependencies(matcherArg);
                scenario.InjectDependencies(matcher);

                return InternalMatch(scenario, matcherArg, matcher);
            }

            private static ThenStep InternalMatch<T>(Scenario scenario, T matcherArg, IMatcher<T> matcher)
            {
                bool passed;
                var diagnostics = new MatchDiagnostics();
                try
                {
                    passed = matcher.Matches(matcherArg, diagnostics);
                }
                catch (Exception e)
                {
                    throw scenario.FailedWithError(e);
                }
                scenario.ThenAssertMatchPassed(passed, matcherArg, matcher, diagnostics);

                return new ThenStep(scenario);
            }

            internal static ThenStep Then(Scenario scenario, Action action)
            {
                scenario.Step("Then(Action)");
                scenario.InjectDependencies(action);
                scenario.InvokeOrFail(action);
                scenario.Passed(true);
                return new ThenStep(scenario);
            }
        }

    }
}