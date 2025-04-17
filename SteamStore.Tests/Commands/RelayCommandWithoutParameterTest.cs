// <copyright file="RelayCommandWithoutParameterTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamStore.ViewModels;

namespace SteamStore.Tests.Commands
{
    public class RelayCommandWithoutParameterTests : IDisposable
    {
        private bool wasExecuteActionCalled;
        private bool wasCanExecuteFunctionCalled;
        private RelayCommandWithoutParameter relayCommand;

        public RelayCommandWithoutParameterTests()
        {
            this.wasExecuteActionCalled = false;
            this.wasCanExecuteFunctionCalled = false;
        }

        [Fact]
        public void Constructor_WhenExecuteActionIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            string expectedStatus = "execute";

            var exception = Assert.Throws<ArgumentNullException>(() => new RelayCommandWithoutParameter(null));

            Assert.Equal(expectedStatus, exception.ParamName);
        }

        [Fact]
        public void Constructor_WhenExecuteActionIsNotNull_AssignsActionCorrectly()
        {
            // Arrange
            Action executeAction = () => this.wasExecuteActionCalled = true;

            // Act
            this.relayCommand = new RelayCommandWithoutParameter(executeAction);
            this.relayCommand.Execute(null);

            // Assert
            Assert.True(this.wasExecuteActionCalled);
        }

        [Fact]
        public void Constructor_WhenValidParametersProvided_InitializesCommandCorrectly()
        {
            // Arrange
            Action executeAction = () => this.wasExecuteActionCalled = true;
            Func<bool> canExecuteFunction = () =>
            {
                this.wasCanExecuteFunctionCalled = true;
                return true;
            };

            // Act
            this.relayCommand = new RelayCommandWithoutParameter(executeAction, canExecuteFunction);

            // Assert
            Assert.NotNull(this.relayCommand);

            this.relayCommand.Execute(null);
            Assert.True(this.wasExecuteActionCalled);

            bool result = this.relayCommand.CanExecute(null);
            Assert.True(this.wasCanExecuteFunctionCalled);
            Assert.True(result);
        }

        [Fact]
        public void CanExecute_WhenNoCanExecuteFunctionProvided_ReturnsTrue()
        {
            // Arrange
            this.relayCommand = new RelayCommandWithoutParameter(() => { });

            // Act
            bool result = this.relayCommand.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanExecute_WhenCanExecuteFunctionReturnsFalse_ReturnsFalse()
        {
            // Arrange
            this.relayCommand = new RelayCommandWithoutParameter(
                () => { },
                () => false);

            // Act
            bool result = this.relayCommand.CanExecute(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_WhenCanExecuteFunctionIsProvided_CallsFunction()
        {
            // Arrange
            this.relayCommand = new RelayCommandWithoutParameter(
                () => { },
                () =>
                {
                    this.wasCanExecuteFunctionCalled = true;
                    return true;
                });

            // Act
            this.relayCommand.CanExecute(null);

            // Assert
            Assert.True(this.wasCanExecuteFunctionCalled);
        }

        [Fact]
        public void Execute_WhenCalled_InvokesAssignedAction()
        {
            // Arrange
            this.relayCommand = new RelayCommandWithoutParameter(() => this.wasExecuteActionCalled = true);

            // Act
            this.relayCommand.Execute(null);

            // Assert
            Assert.True(this.wasExecuteActionCalled);
        }

        [Fact]
        public void RaiseCanExecuteChanged_WhenCalled_InvokesEventIfSubscribed()
        {
            // Arrange
            bool wasEventRaised = false;
            this.relayCommand = new RelayCommandWithoutParameter(() => { });
            this.relayCommand.CanExecuteChanged += (sender, arguments) => wasEventRaised = true;

            // Act
            this.relayCommand.RaiseCanExecuteChanged();

            // Assert
            Assert.True(wasEventRaised);
        }

        [Fact]
        public void RaiseCanExecuteChanged_WhenNoSubscribers_DoesNotThrowException()
        {
            // Arrange
            this.relayCommand = new RelayCommandWithoutParameter(() => { });

            // Act & Assert
            var exception = Record.Exception(() => this.relayCommand.RaiseCanExecuteChanged());

            Assert.Null(exception);
        }

        [Fact]
        public void CanExecuteChanged_WhenHandlerIsSubscribedAndUnsubscribed_EventIsRaisedAndThenNotRaised()
        {
            // Arrange
            bool wasEventRaised = false;
            EventHandler handler = (sender, args) => wasEventRaised = true;
            this.relayCommand = new RelayCommandWithoutParameter(() => { });

            // Act
            this.relayCommand.CanExecuteChanged += handler;
            this.relayCommand.RaiseCanExecuteChanged();
            bool afterFirstRaise = wasEventRaised;

            wasEventRaised = false;
            this.relayCommand.CanExecuteChanged -= handler;
            this.relayCommand.RaiseCanExecuteChanged();
            bool afterSecondRaise = wasEventRaised;

            // Assert
            Assert.True(afterFirstRaise);
            Assert.False(afterSecondRaise);
        }

        [Fact]
        public void Execute_WhenParameterProvided_StillExecutesAction()
        {
            // Arrange
            this.relayCommand = new RelayCommandWithoutParameter(() => this.wasExecuteActionCalled = true);

            // Act
            this.relayCommand.Execute("some parameter");

            // Assert
            Assert.True(this.wasExecuteActionCalled);
        }

        [Fact]
        public void CanExecute_WhenParameterProvided_StillCallsCanExecuteFunction()
        {
            // Arrange
            this.relayCommand = new RelayCommandWithoutParameter(
                () => { },
                () =>
                {
                    this.wasCanExecuteFunctionCalled = true;
                    return true;
                });

            // Act
            this.relayCommand.CanExecute("some parameter");

            // Assert
            Assert.True(this.wasCanExecuteFunctionCalled);
        }

        public void Dispose()
        {
            this.relayCommand = null;
        }
    }
}
