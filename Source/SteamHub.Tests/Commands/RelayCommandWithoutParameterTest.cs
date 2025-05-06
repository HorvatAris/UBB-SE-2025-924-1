// <copyright file="RelayCommandWithoutParameterTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Tests.Commands
{
    using System;
    using SteamHub.ViewModels;
    using Xunit;

    public class RelayCommandWithoutParameterTest : IDisposable
    {
        private bool wasExecuteActionCalled;
        private bool wasCanExecuteFunctionCalled;
        private RelayCommandWithoutParameter relayCommand;

        public RelayCommandWithoutParameterTest()
        {
            this.wasExecuteActionCalled = false;
            this.wasCanExecuteFunctionCalled = false;
        }

        [Fact]
        public void Constructor_WhenExecuteActionIsNull_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new RelayCommandWithoutParameter(null));

            Assert.Equal("execute", exception.ParamName);
        }

        [Fact]
        public void Constructor_WhenExecuteActionIsValid_AssignsActionCorrectly()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => this.wasExecuteActionCalled = true);

            this.relayCommand.Execute(null);

            Assert.True(this.wasExecuteActionCalled);
        }

        [Fact]
        public void Constructor_WhenCanExecuteProvided_InitializesCommand()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => this.wasExecuteActionCalled = true, () => true);

            Assert.NotNull(this.relayCommand);
        }

        [Fact]
        public void Execute_WhenCalled_TriggersExecuteAction()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => this.wasExecuteActionCalled = true);

            this.relayCommand.Execute(null);

            Assert.True(this.wasExecuteActionCalled);
        }

        [Fact]
        public void CanExecute_WhenNoPredicate_ReturnsTrue()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => { });

            bool result = this.relayCommand.CanExecute(null);

            Assert.True(result);
        }

        [Fact]
        public void CanExecute_WhenPredicateReturnsFalse_ReturnsFalse()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => { }, () => false);

            bool result = this.relayCommand.CanExecute(null);

            Assert.False(result);
        }

        [Fact]
        public void CanExecute_WhenPredicateIsCalled_SetsFlag()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => { }, () =>
            {
                this.wasCanExecuteFunctionCalled = true;
                return true;
            });

            this.relayCommand.CanExecute(null);

            Assert.True(this.wasCanExecuteFunctionCalled);
        }

        [Fact]
        public void RaiseCanExecuteChanged_WhenSubscribed_RaisesEvent()
        {
            bool wasRaised = false;

            this.relayCommand = new RelayCommandWithoutParameter(() => { });
            this.relayCommand.CanExecuteChanged += (sender, eventArguments) => wasRaised = true;
            this.relayCommand.RaiseCanExecuteChanged();

            Assert.True(wasRaised);
        }

        [Fact]
        public void RaiseCanExecuteChanged_WhenNoSubscribers_DoesNotThrow()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => { });

            var exception = Record.Exception(() => this.relayCommand.RaiseCanExecuteChanged());

            Assert.Null(exception);
        }

        [Fact]
        public void CanExecuteChanged_WhenHandlerSubscribedAndUnsubscribed_BehavesAsExpected_FirstRaiseTrue()
        {
            bool wasRaised = false;

            EventHandler handler = (sender, eventArguments) => wasRaised = true;
            this.relayCommand = new RelayCommandWithoutParameter(() => { });
            this.relayCommand.CanExecuteChanged += handler;
            this.relayCommand.RaiseCanExecuteChanged();

            Assert.True(wasRaised);
        }

        [Fact]
        public void CanExecuteChanged_WhenHandlerSubscribedAndUnsubscribed_BehavesAsExpected_SecondRaiseFalse()
        {
            bool wasRaised = false;

            EventHandler handler = (sender, eventArguments) => wasRaised = true;
            this.relayCommand = new RelayCommandWithoutParameter(() => { });
            this.relayCommand.CanExecuteChanged += handler;
            this.relayCommand.CanExecuteChanged -= handler;
            this.relayCommand.RaiseCanExecuteChanged();

            Assert.False(wasRaised);
        }

        [Fact]
        public void Execute_WithObjectParameter_StillCallsExecute()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => this.wasExecuteActionCalled = true);

            this.relayCommand.Execute("test");

            Assert.True(this.wasExecuteActionCalled);
        }

        [Fact]
        public void CanExecute_WithObjectParameter_StillCallsPredicate()
        {
            this.relayCommand = new RelayCommandWithoutParameter(() => { }, () =>
            {
                this.wasCanExecuteFunctionCalled = true;
                return true;
            });

            this.relayCommand.CanExecute("parameters");

            Assert.True(this.wasCanExecuteFunctionCalled);
        }

        public void Dispose()
        {
            this.relayCommand = null;
        }
    }
}
