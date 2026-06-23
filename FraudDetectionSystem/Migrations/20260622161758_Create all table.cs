using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FraudDetectionSystem.Migrations
{
    /// <inheritdoc />
    public partial class Createalltable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "FraudProbabilityPercent",
                table: "Transactions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "HasOfferApplied",
                table: "Transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFraud",
                table: "Transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ItemCategory",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CloseHour",
                table: "Stores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Stores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OpenHour",
                table: "Stores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousStoreType",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreType",
                table: "Stores",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Stores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StoreReturnMonitorings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FrequentReturnCustomerCount",
                table: "StoreReturnMonitorings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StoreReturnMonitorings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "StoreFraudAlerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StoreFraudAlerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "FraudProbabilityPercent",
                table: "StoreFraudAlerts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsFraud",
                table: "StoreFraudAlerts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StoreFraudAlerts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StoreFraudAlerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StoreDailySales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DayType",
                table: "StoreDailySales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StoreDailySales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "FraudAlerts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "FraudAlerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FraudAlerts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "FraudAlerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "FraudAlerts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "FraudProbabilityPercent",
                table: "FraudAlerts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsFraud",
                table: "FraudAlerts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "FraudAlerts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "FraudAlerts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FraudAlerts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "FraudAlerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "AveragePurchase",
                table: "Customers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "CoinPurchaseRatio",
                table: "Customers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "DiamondPurchaseRatio",
                table: "Customers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstVisitDate",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "GoldPurchaseRatio",
                table: "Customers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceCount",
                table: "Customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHni",
                table: "Customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "JewelleryPurchaseRatio",
                table: "Customers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "LifetimeValue",
                table: "Customers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ReturnCount",
                table: "Customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VisitCount",
                table: "Customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreThresholds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoreId = table.Column<int>(type: "integer", nullable: false),
                    DayType = table.Column<int>(type: "integer", nullable: false),
                    SalesThreshold = table.Column<decimal>(type: "numeric", nullable: false),
                    ReturnCountThreshold = table.Column<int>(type: "integer", nullable: false),
                    ReturnValueThreshold = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreThresholds", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "StoreThresholds");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FraudProbabilityPercent",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "HasOfferApplied",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsFraud",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ItemCategory",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CloseHour",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "OpenHour",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PreviousStoreType",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreType",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StoreReturnMonitorings");

            migrationBuilder.DropColumn(
                name: "FrequentReturnCustomerCount",
                table: "StoreReturnMonitorings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StoreReturnMonitorings");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "StoreFraudAlerts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StoreFraudAlerts");

            migrationBuilder.DropColumn(
                name: "FraudProbabilityPercent",
                table: "StoreFraudAlerts");

            migrationBuilder.DropColumn(
                name: "IsFraud",
                table: "StoreFraudAlerts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StoreFraudAlerts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StoreFraudAlerts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StoreDailySales");

            migrationBuilder.DropColumn(
                name: "DayType",
                table: "StoreDailySales");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StoreDailySales");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "FraudProbabilityPercent",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "IsFraud",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "AveragePurchase",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CoinPurchaseRatio",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DiamondPurchaseRatio",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirstVisitDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "GoldPurchaseRatio",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "InvoiceCount",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsHni",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "JewelleryPurchaseRatio",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LifetimeValue",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ReturnCount",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VisitCount",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "FraudAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
