namespace SteamStore.Tests.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamStore.ViewModels;

    public class RelayCommandTest : IDisposable
    {
        private bool executeCalled;
        private bool canExecuteCalled;
        private string capturedParameter;
        private RelayCommand<string> stringRelayCommand;

        public RelayCommandTest()
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
            int someIntegerValue = 42;
            var intCommand = new RelayCommand<int>((parameter) => { });

            bool result = intCommand.CanExecute(someIntegerValue);

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
        public void CanExecuteChanged_WhenHandlerSubscribed_EventFires()
        {
            bool eventRaised = false;
            EventHandler handler = (sender, arguments) => eventRaised = true;

            this.stringRelayCommand = new RelayCommand<string>(parameter => { });
            this.stringRelayCommand.CanExecuteChanged += handler;
            this.stringRelayCommand.RaiseCanExecuteChanged();

            Assert.True(eventRaised);
        }

        [Fact]
        public void CanExecuteChanged_WhenHandlerUnsubscribed_EventDoesNotFire()
        {
            bool eventRaised = false;
            EventHandler handler = (sender, arguments) => eventRaised = true;

            this.stringRelayCommand = new RelayCommand<string>(parameter => { });
            this.stringRelayCommand.CanExecuteChanged += handler;
            this.stringRelayCommand.CanExecuteChanged -= handler;
            this.stringRelayCommand.RaiseCanExecuteChanged();

            Assert.False(eventRaised);
        }

        [Fact]
        public void CanExecute_WithExpectedCondition_ReturnsTrue()
        {
            this.stringRelayCommand = new RelayCommand<string>(
                parameter => { },
                parameter => parameter == "valid");

            bool result = this.stringRelayCommand.CanExecute("valid");

            Assert.True(result);
        }

        [Fact]
        public void CanExecute_WithUnexpectedCondition_ReturnsFalse()
        {
            this.stringRelayCommand = new RelayCommand<string>(
                parameter => { },
                parameter => parameter == "valid");

            bool result = this.stringRelayCommand.CanExecute("invalid");

            Assert.False(result);
        }

        public void Dispose()
        {
            this.stringRelayCommand = null;
        }
    }
}
