using System;
using System.ComponentModel;
using NUnit.Framework;
using TestFirst.Net.Extensions.Moq;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Extensions.Test.Moq
{
    [TestFixture]
    public class RaiseEventTest : AbstractNUnitMoqScenarioTest
    {
        [Test]
        public void FiresPropertyChanged()
        {
            INotifyPropertyChanged viewModel;
            IPropertyChangedMethod handler;

            Scenario()
                .Given(viewModel = AMock<INotifyPropertyChanged>().Instance)
                .Given(handler = AMock<IPropertyChangedMethod>()
                    .WhereMethod(h => h.Fires(ArgIs(AnInstance.SameAs(viewModel)), ArgIs(APropertyChangedEventArgs.With().PropertyName("SomeProperty"))))
                        .IsCalled(1).Times()
                    .Instance)
                .Given(() => viewModel.PropertyChanged += handler.Fires)

                .When(() => viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(viewModel, new PropertyChangedEventArgs("SomeProperty")))

                .Then(ExpectNoMocksFailed());
        }

        [Test]
        public void FiresEventHandler()
        {
            ITestInterface viewModel;
            IEventHandlerMethod<TestEventArgs> handler;
            TestEventArgs args;

            Scenario()
                .Given(viewModel = AMock<ITestInterface>().Instance)
                .Given(args = new TestEventArgs())
                .Given(handler = AMock<IEventHandlerMethod<TestEventArgs>>()
                    .WhereMethod(h => h.Fires(ArgIs(AnInstance.SameAs(viewModel)), ArgIs(AnInstance.SameAs(args))))
                        .IsCalled(1).Times()
                    .Instance)
                .Given(() => viewModel.SomeEvent += handler.Fires)

                .When(() => viewModel.SomeEvent += Raise.Event(viewModel, args))

                .Then(ExpectNoMocksFailed());
        }

        [Test]
        public void FiresMultipleEvents()
        {
            ITestInterface viewModel;
            IEventHandlerMethod<TestEventArgs> eventHandler;
            TestEventArgs args;
            IPropertyChangedMethod propertyChangedHandler;

            Scenario()
                .Given(viewModel = AMock<ITestInterface>().Instance)
                .Given(args = new TestEventArgs())
                .Given(eventHandler = AMock<IEventHandlerMethod<TestEventArgs>>()
                    .WhereMethod(h => h.Fires(ArgIs(AnInstance.SameAs(viewModel)), ArgIs(AnInstance.SameAs(args))))
                        .IsCalled(1).Times()
                    .Instance)
                .Given(propertyChangedHandler = AMock<IPropertyChangedMethod>()
                    .WhereMethod(h => h.Fires(ArgIs(AnInstance.SameAs(viewModel)), ArgIs(APropertyChangedEventArgs.With().PropertyName("SomeProperty"))))
                        .IsCalled(1).Times()
                    .Instance)
                .Given(() => viewModel.SomeEvent += eventHandler.Fires)
                .Given(() => viewModel.PropertyChanged += propertyChangedHandler.Fires)

                .When(() => viewModel.SomeEvent += Raise.Event(viewModel, args))
                .When(() => viewModel.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(viewModel, new PropertyChangedEventArgs("SomeProperty")))

                .Then(ExpectNoMocksFailed());
        }

        [Test]
        public void FiresEventHandlerMultipleTimes()
        {
            ITestInterface viewModel;
            IEventHandlerMethod<TestEventArgs> handler;
            TestEventArgs args;

            Scenario()
                .Given(viewModel = AMock<ITestInterface>().Instance)
                .Given(args = new TestEventArgs())
                .Given(handler = AMock<IEventHandlerMethod<TestEventArgs>>()
                    .WhereMethod(h => h.Fires(ArgIs(AnInstance.SameAs(viewModel)), ArgIs(AnInstance.SameAs(args))))
                        .IsCalled(2).Times()
                    .Instance)
                .Given(() => viewModel.SomeEvent += handler.Fires)

                .When(() => viewModel.SomeEvent += Raise.Event(viewModel, args))
                .When(() => viewModel.SomeEvent += Raise.Event(viewModel, args))

                .Then(ExpectNoMocksFailed());
        }

        [Test]
        public void RaiseAfterRemoveDoesNotFireHandler()
        {
            ITestInterface viewModel;
            IEventHandlerMethod<TestEventArgs> handler;
            TestEventArgs args;

            Scenario()
                .Given(viewModel = AMock<ITestInterface>().Instance)
                .Given(args = new TestEventArgs())
                .Given(handler = AMock<IEventHandlerMethod<TestEventArgs>>()
                    .WhereMethod(h => h.Fires(ArgIs(AnInstance.SameAs(viewModel)), ArgIs(AnInstance.SameAs(args))))
                        .IsCalled(1).Times()
                    .Instance)
                .Given(() => viewModel.SomeEvent += handler.Fires)

                .Given(() => viewModel.SomeEvent += Raise.Event(viewModel, args))

                .When(() => viewModel.SomeEvent -= handler.Fires)
                .When(() => viewModel.SomeEvent += Raise.Event(viewModel, args))

                .Then(ExpectNoMocksFailed());
        }

        [Test]
        public void RemoveWithoutAddingDoesNothing()
        {
            ITestInterface viewModel;
            IEventHandlerMethod<TestEventArgs> handler;
            TestEventArgs args;

            Scenario()
                .Given(viewModel = AMock<ITestInterface>().Instance)
                .Given(args = new TestEventArgs())
                .Given(handler = AMock<IEventHandlerMethod<TestEventArgs>>().Instance)

                .Given(() => viewModel.SomeEvent += Raise.Event<EventHandler<TestEventArgs>>(viewModel, args))

                .When(() => viewModel.SomeEvent -= handler.Fires)
                .When(() => viewModel.SomeEvent += Raise.Event<EventHandler<TestEventArgs>>(viewModel, args))

                .Then(ExpectNoMocksFailed());
        }
    }

    public class TestEventArgs : EventArgs
    {
    }

    internal class APropertyChangedEventArgs : PropertyMatcher<PropertyChangedEventArgs>
    {
        private static readonly PropertyChangedEventArgs PropertyNames = null;

        private APropertyChangedEventArgs()
        {
        }

        public static APropertyChangedEventArgs With()
        {
            return new APropertyChangedEventArgs();
        }

        public APropertyChangedEventArgs PropertyName(string propertyName)
        {
            return PropertyName(AString.EqualTo(propertyName));
        }

        public APropertyChangedEventArgs PropertyName(IMatcher<string> propertyName)
        {
            WithProperty(() => PropertyNames.PropertyName, propertyName);
            return this;
        }
    }
}
