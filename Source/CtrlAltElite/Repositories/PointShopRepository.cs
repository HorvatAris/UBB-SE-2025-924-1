// <copyright file="PointShopRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using CtrlAltElite.Models;
    using SteamStore.Constants;
    using SteamStore.Data;
    using SteamStore.Models;
    using SteamStore.Repositories.Interfaces;

    public class PointShopRepository : IPointShopRepository
    {
        private User user;
        private IDataLink data;

        public PointShopRepository(User user, IDataLink data)
        {
            this.user = user;
            this.data = data;
        }

        public User GetCurrentUser()
        {
            return this.user;
        }

        public List<PointShopItem> GetAllItems()
        {
            var pointShopItems = new List<PointShopItem>();

            try
            {
                DataTable result = this.data.ExecuteReader(SqlConstants.GetAllPointShopItems);

                foreach (DataRow row in result.Rows)
                {
                    var item = new PointShopItem
                    {
                        ItemIdentifier = Convert.ToInt32(row[SqlConstants.ItemIdColumnWithCapitalLetter]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"].ToString(),
                        ImagePath = row[SqlConstants.ImagePathColumnWithCapitalLetter].ToString(),
                        PointPrice = Convert.ToDouble(row[SqlConstants.PointPriceColumnWithCapitalLeter]),
                        ItemType = row[SqlConstants.ItemTypeColumnWithCapitalLetter].ToString(),
                    };

                    pointShopItems.Add(item);
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to retrieve point shop items: {exception.Message}");
            }

            return pointShopItems;
        }

        public List<PointShopItem> GetUserItems()
        {
            var userPointShopItems = new List<PointShopItem>();

            try
            {
                SqlParameter[] userParameters = new SqlParameter[]
                {
                    new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, this.user.UserId),
                };

                DataTable result = this.data.ExecuteReader(SqlConstants.GetUserPointShopItemsProcedure, userParameters);

                foreach (DataRow row in result.Rows)
                {
                    var currentItem = new PointShopItem
                    {
                        ItemIdentifier = Convert.ToInt32(row[SqlConstants.ItemIdColumnWithCapitalLetter]),
                        Name = row["Name"].ToString(),

                       // Name = row[SqlConstants.NameIdColumnWithCapitalLetter].ToString(),
                        Description = row["Description"].ToString(),
                        ImagePath = row[SqlConstants.ImagePathColumnWithCapitalLetter].ToString(),
                        PointPrice = Convert.ToDouble(row[SqlConstants.PointPriceColumnWithCapitalLeter]),
                        ItemType = row[SqlConstants.ItemTypeColumnWithCapitalLetter].ToString(),
                        IsActive = Convert.ToBoolean(row[SqlConstants.IsActiveColumn]),
                    };
                    userPointShopItems.Add(currentItem);
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to retrieve user's point shop items: {exception.Message}");
            }

            System.Diagnostics.Debug.WriteLine("User point shop items are" + userPointShopItems);
            return userPointShopItems;
        }

        public void PurchaseItem(PointShopItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Cannot purchase a null item");
            }

            if (this.user == null)
            {
                throw new InvalidOperationException("User is not initialized");
            }

            if (this.user.PointsBalance < item.PointPrice)
            {
                throw new Exception("Insufficient points to purchase this item");
            }

            try
            {
                SqlParameter[] pointShopPurchaseParameters = new SqlParameter[]
                {
                    new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, this.user.UserId),
                    new SqlParameter(SqlConstants.ItemIdParameter, item.ItemIdentifier),
                };

                this.data.ExecuteNonQuery(SqlConstants.PurchasePointShopItemProcedure, pointShopPurchaseParameters);

                this.user.PointsBalance -= (float)item.PointPrice;

                this.UpdateUserPointBalance();
            }
            catch (Exception exception)
            {
                 throw new Exception($"Failed to purchase item: {exception.Message}");
            }
        }

        public void ActivateItem(PointShopItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Cannot activate a null item");
            }

            if (this.user == null)
            {
                throw new InvalidOperationException("User is not initialized");
            }

            try
            {
                SqlParameter[] activateItemParameters = new SqlParameter[]
                {
                    new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, this.user.UserId),
                    new SqlParameter(SqlConstants.ItemIdParameter, item.ItemIdentifier),
                };

                this.data.ExecuteNonQuery(SqlConstants.ActivatePointSHopIntemProcedure, activateItemParameters);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to activate item: {exception.Message}");
            }
        }

        public void DeactivateItem(PointShopItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Cannot deactivate a null item");
            }

            if (this.user == null)
            {
                throw new InvalidOperationException("User is not initialized");
            }

            try
            {
                SqlParameter[] deactivateItemParameters = new SqlParameter[]
                {
                    new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, this.user.UserId),
                    new SqlParameter(SqlConstants.ItemIdParameter, item.ItemIdentifier),
                };

                this.data.ExecuteNonQuery(SqlConstants.DeactivatePointShopItemProcedure, deactivateItemParameters);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to deactivate item: {exception.Message}");
            }
        }

        public void UpdateUserPointBalance()
        {
            try
            {
                SqlParameter[] userPointBalanceParametrs = new SqlParameter[]
                {
                    new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, this.user.UserId),
                    new SqlParameter(SqlConstants.PointBalanceParameter, this.user.PointsBalance),
                };

                this.data.ExecuteNonQuery(SqlConstants.UpdateUserPointBalance, userPointBalanceParametrs);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to update user point balance: {exception.Message}");
            }
        }

        public void ResetUserInventory()
        {
            if (this.user == null)
            {
                throw new InvalidOperationException("User is not initialized");
            }

            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(SqlConstants.UserIdParameterWithCapitalLetter, this.user.UserId),
                };
                this.data.ExecuteNonQuery(SqlConstants.ResetUserInventoryToDefault, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove item from user: {ex.Message}");
            }
        }
    }
}