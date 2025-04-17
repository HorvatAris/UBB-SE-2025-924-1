using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamStore.ViewModels;

namespace SteamStore.Tests.Commands
{
    public class RelayCommandTests : IDisposable
    {
        private bool executeCalled;
        private bool canExecuteCalled;
        private string capturedParameter;
        private RelayCommand<string> stringRelayCommand;

        public RelayCommandTests()
        {
            this.executeCalled = false;
            this.canExecuteCalled = false;
            this.capturedParameter = null;
        }

        [Fact]
        public void Constructor_WhenExecuteActionIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RelayCommand<string>(null));
        }

        [Fact]
        public void Constructor_WhenValidParameters_CommandIsInitializedCorrectly()
        {
            Action<string> executeAction = (parameter) =>
            {
                this.executeCalled = true;
                this.capturedParameter = parameter;
            };
            Predicate<string> canExecutePredicate = (parameter) =>
            {
                this.canExecuteCalled = true;
                return true;
            };

            this.stringRelayCommand = new RelayCommand<string>(executeAction, canExecutePredicate);

            Assert.NotNull(this.stringRelayCommand);
        }

        [Fact]
        public void CanExecute_WhenNoCanExecuteFunctionProvided_ReturnsTrue()
        {
            this.stringRelayCommand = new RelayCommand<string>((parameter) => { });

            bool result = this.stringRelayCommand.CanExecute("test");

            Assert.True(result);
        }

        [Fact]
        public void CanExecute_WhenNullParameterPassedForValueType_ReturnsTrue()
        {
            var intCommand = new RelayCommand<int>((parameter) => { });

            bool result = intCommand.CanExecute(null);

            Assert.True(result);
        }

        [Fact]
        public void CanExecute_WhenValidValueTypeParameterProvided_ReturnsTrue()
        {
            var intCommand = new RelayCommand<int>((parameter) => { });

            bool result = intCommand.CanExecute(42);

            Assert.True(result);
        }

        [Fact]
        public void CanExecute_WhenCanExecuteFunctionProvided_InvokesFunction()
        {
            this.stringRelayCommand = new RelayCommand<string>(
                (parameter) => { },
                (parameter) =>
                {
                    this.canExecuteCalled = true;
                    return true;
                });

            this.stringRelayCommand.CanExecute("test");

            Assert.True(this.canExecuteCalled);
        }

        [Fact]
        public void Execute_WhenCalled_ExecutesProvidedActionWithParameter()
        {
            const string testParameter = "test parameter";

            this.stringRelayCommand = new RelayCommand<string>((parameter) =>
            {
                this.executeCalled = true;
                this.capturedParameter = parameter;
            });

            this.stringRelayCommand.Execute(testParameter);

            Assert.True(this.executeCalled);
            Assert.Equal(testParameter, this.capturedParameter);
        }

        [Fact]
        public void RaiseCanExecuteChanged_WhenInvoked_RaisesCanExecuteChangedEvent()
        {
            bool eventRaised = false;
            
            this.stringRelayCommand = new RelayCommand<string>((parametr) => { });
            this.stringRelayCommand.CanExecuteChanged += (sender, arguments) => eventRaised = true;
            this.stringRelayCommand.RaiseCanExecuteChanged();

            Assert.True(eventRaised);
        }

        [Fact]
        public void CanExecuteChanged_WhenSubscribedAndUnsubscribed_EventIsRaisedOrSuppressed()
        {
            bool eventRaised = false;

            EventHandler handler = (sender, arguments) => eventRaised = true;
            this.stringRelayCommand = new RelayCommand<string>((parameters) => { });

            this.stringRelayCommand.CanExecuteChanged += handler;
            this.stringRelayCommand.RaiseCanExecuteChanged();
            bool firstCall = eventRaised;

            eventRaised = false;
            this.stringRelayCommand.CanExecuteChanged -= handler;
            this.stringRelayCommand.RaiseCanExecuteChanged();
            bool secondCall = eventRaised;

            Assert.True(firstCall);
            Assert.False(secondCall);
        }

        [Fact]
        public void CanExecute_WhenConditionMatches_ReturnsExpectedResult()
        {
            this.stringRelayCommand = new RelayCommand<string>(
                (parameters) => { },
                (parameters) => parameters == "valid");

            bool validResult = this.stringRelayCommand.CanExecute("valid");
            bool invalidResult = this.stringRelayCommand.CanExecute("invalid");

            Assert.True(validResult);
            Assert.False(invalidResult);
        }

        public void Dispose()
        {
            this.stringRelayCommand = null;
        }
    }
}
