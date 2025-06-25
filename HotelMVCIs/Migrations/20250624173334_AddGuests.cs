using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelMVCIs.Migrations
{
    /// <inheritdoc />
    public partial class AddGuests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "Address", "City", "DateOfBirth", "Email", "FirstName", "LastName", "Nationality", "PhoneNumber", "PostalCode" },
                values: new object[,]
                {
                    { 3, "Václavské náměstí 1", "Praha", new DateTime(1985, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "jan.novak@example.com", "Jan", "Novák", "Česká republika", "123 456 789", "110 00" },
                    { 4, "Náměstí Svobody 15", "Brno", new DateTime(1992, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "eva.dvorakova@example.com", "Eva", "Dvořáková", "Česká republika", "987 654 321", "602 00" },
                    { 5, "Masarykovo náměstí 5", "Ostrava", new DateTime(1978, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "petr.cerny@example.com", "Petr", "Černý", "Česká republika", "555 111 222", "702 00" },
                    { 6, "Americká 42", "Plzeň", new DateTime(2001, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "jana.svobodova@example.com", "Jana", "Svobodová", "Česká republika", "444 555 666", "301 00" },
                    { 7, "10 Downing Street", "London", new DateTime(1990, 7, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.smith@example.co.uk", "John", "Smith", "United Kingdom", "+44 20 7946 0958", "SW1A 2AA" },
                    { 8, "Gran Vía 28", "Madrid", new DateTime(1988, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "maria.garcia@example.es", "Maria", "Garcia", "Spain", "+34 917 123 456", "28013" },
                    { 9, "Unter den Linden 77", "Berlin", new DateTime(1982, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "hans.muller@example.de", "Hans", "Müller", "Germany", "+49 30 1234567", "10117" },
                    { 10, "Nowy Świat 1", "Warsaw", new DateTime(1995, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "anna.kowalska@example.pl", "Anna", "Kowalska", "Poland", "+48 22 123 45 67", "00-400" },
                    { 11, "Via del Corso 1", "Rome", new DateTime(1975, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "luca.rossi@example.it", "Luca", "Rossi", "Italy", "+39 06 12345678", "00186" },
                    { 12, "Pražská 123", "Liberec", new DateTime(1999, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "martin.prochazka@example.com", "Martin", "Procházka", "Česká republika", "777 888 999", "460 01" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
