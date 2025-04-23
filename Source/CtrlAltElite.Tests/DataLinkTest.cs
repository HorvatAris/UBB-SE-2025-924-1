// <copyright file="DataLinkTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Data;
using System.Data.SqlClient;
using SteamStore.Data;
using SteamStore.Constants;
using SteamStore.Tests.TestUtils;
using Xunit;

namespace SteamStore.Tests.Data
{
    public class DataLinkTests
    {
        private readonly IDataLink dataLink;

        private const string UnsupportedProcedureName = "UnsupportedProcedure";
        private const string ValidProcedureWithScalarResult = "GetGameOwnerCount";
        private const string ArbitraryProcedure = "AnyProcedure";
        private const string InvalidSqlParameterName = "@InvalidParameter";
        private const string InvalidSqlParameterValue = "InvalidValue";
        private const string ExpectedExecuteReaderError = "Error - ExecuteReader";
        private const string ExpectedExecuteNonQueryError = "Error - ExecuteNonQuery";
        private const string ExpectedExecuteScalarError = "Error - ExecutingScalar";
        private const int EmptyReturnedRowsCount = 0;
        private const int DefaultExpectedCount = 0;
        private const int MinimumRatingThreshold = 0;
        private const int MaximumRatingThreshold = 5;
        private const int ExistingUserId = 1;
        private const int ExistingGameId = 1;

        public DataLinkTests()
        {
            this.dataLink = DataLinkTestUtils.GetDataLink();
        }

        [Fact]
        public void OpenConnection_WhenCalled_DoesNotThrow()
        {
            this.dataLink.OpenConnection();

            Assert.True(true);
        }

        [Fact]
        public void CloseConnection_WhenCalled_DoesNotThrow()
        {
            this.dataLink.CloseConnection();

            Assert.True(true);
        }

        [Fact]
        public void ExecuteReader_WithNullParameters_ThrowsExpectedException()
        {
            var exception = Assert.Throws<Exception>(() =>
                this.dataLink.ExecuteReader(SqlConstants.GetUserGamesProcedure, null));

            Assert.Contains(ExpectedExecuteReaderError, exception.Message);
        }

        [Fact]
        public void ExecuteReader_WithEmptyParameters_ThrowsExpectedException()
        {
            var exception = Assert.Throws<Exception>(() =>
                this.dataLink.ExecuteReader(SqlConstants.GetUserGamesProcedure, new SqlParameter[] { }));

            Assert.Contains(ExpectedExecuteReaderError, exception.Message);
        }

        [Fact]
        public void ExecuteReader_WithInvalidParameters_ThrowsExpectedException()
        {
            var parameters = new[]
            {
                new SqlParameter(InvalidSqlParameterName, InvalidSqlParameterValue)
            };

            var exception = Assert.Throws<Exception>(() =>
                this.dataLink.ExecuteReader(SqlConstants.GetUserGamesProcedure, parameters));

            Assert.Contains(ExpectedExecuteReaderError, exception.Message);
        }

        [Fact]
        public void ExecuteReader_WithUnsupportedProcedure_ThrowsExpectedException()
        {
            var exception = Assert.Throws<Exception>(() =>
                this.dataLink.ExecuteReader(UnsupportedProcedureName));

            Assert.Contains(ExpectedExecuteReaderError, exception.Message);
        }

        [Fact]
        public void ExecuteNonQuery_WithUnsupportedProcedure_ThrowsExpectedException()
        {
            var exception = Assert.Throws<Exception>(() =>
                this.dataLink.ExecuteNonQuery(ArbitraryProcedure));

            Assert.Contains(ExpectedExecuteNonQueryError, exception.Message);
        }

        [Fact]
        public void ExecuteScalar_WithUnsupportedProcedure_ThrowsExpectedException()
        {
            var exception = Assert.Throws<Exception>(() =>
                this.dataLink.ExecuteScalar<int>(ArbitraryProcedure));

            Assert.Contains(ExpectedExecuteScalarError, exception.Message);
        }

        [Fact]
        public void ExecuteReader_GetUserByIdProcedure_ReturnsTableWithUserIdColumn()
        {
            var parameters = new[]
            {
                new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, SqlDbType.Int) { Value = ExistingUserId }
            };

            DataTable result = this.dataLink.ExecuteReader(SqlConstants.GetUserByIdProcedure, parameters);

            Assert.True(result.Columns.Contains(SqlConstants.UserIdColumn));
        }

        [Fact]
        public void ExecuteReader_GetUserByIdProcedure_ReturnsAtLeastOneRow()
        {
            var parameters = new[]
            {
                new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, SqlDbType.Int) { Value = ExistingUserId }
            };

            DataTable result = this.dataLink.ExecuteReader(SqlConstants.GetUserByIdProcedure, parameters);

            Assert.True(result.Rows.Count > EmptyReturnedRowsCount);
        }

        [Fact]
        public void ExecuteReader_GetUserGamesProcedure_ContainsExpectedGameIdColumn()
        {
            var parameters = new[]
            {
                new SqlParameter(SqlConstants.UserIdentifierParameter, SqlDbType.Int) { Value = ExistingUserId }
            };

            DataTable result = this.dataLink.ExecuteReader(SqlConstants.GetUserGamesProcedure, parameters);

            Assert.True(result.Columns.Contains(SqlConstants.GAMEIDCOLUMN));
        }

        [Fact]
        public void ExecuteReader_GetAllTagsProcedure_ContainsExpectedTagIdColumn()
        {
            DataTable result = this.dataLink.ExecuteReader(SqlConstants.GetAllTagsProcedure);

            Assert.True(result.Columns.Contains(SqlConstants.TagIdColumn));
        }

        [Fact]
        public void ExecuteReader_GetGameTagsProcedure_ContainsExpectedTagIdColumn()
        {
            var parameters = new[]
            {
                new SqlParameter(SqlConstants.GameIdShortcutParameter, SqlDbType.Int) { Value = ExistingGameId }
            };

            DataTable result = this.dataLink.ExecuteReader(SqlConstants.GetGameTagsProcedure, parameters);

            Assert.True(result.Columns.Contains(SqlConstants.TagIdColumn));
        }

        [Fact]
        public void ExecuteScalar_GetGameOwnerCountProcedure_ReturnsInteger()
        {
            var parameters = new[]
            {
                new SqlParameter(SqlConstants.GameIdParameter, SqlDbType.Int) { Value = ExistingGameId }
            };

            int result = this.dataLink.ExecuteScalar<int>(ValidProcedureWithScalarResult, parameters);

            Assert.IsType<int>(result);
        }

        [Fact]
        public void ExecuteScalar_GetGameRatingProcedure_ReturnsRatingInExpectedRange()
        {
            var parameters = new[]
            {
                new SqlParameter(SqlConstants.GameIdShortcutParameter, SqlDbType.Int) { Value = ExistingGameId }
            };

            float result = this.dataLink.ExecuteScalar<float>(SqlConstants.GetGameRatingProcedure, parameters);

            Assert.InRange(result, MinimumRatingThreshold, MaximumRatingThreshold);
        }
    }
}
