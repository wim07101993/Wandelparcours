using System;
using System.Collections.Generic;
using MongoDB.Bson;
using WebService.Models;
using WebService.Services.Randomizer;

namespace WebService.Tests.TestModels
{
    public static class TestData
    {
        public static List<TestEntity> TestEntities { get; } = new List<TestEntity>
        {
            new TestEntity {B = true, I = 64, Id = ObjectId.GenerateNewId(), S = "Hello"},
            new TestEntity {B = true, I = 44, Id = ObjectId.GenerateNewId(), S = "Bumbabelu"},
            new TestEntity {B = false, I = 42, Id = ObjectId.GenerateNewId(), S = "Bam"},
        };

        public static List<Resident> TestResidents { get; } = new List<Resident>
        {
            new Resident
            {
                Id = new ObjectId("5a9566c58b9ed54db08d0ce7"),
                FirstName = "Lea",
                LastName = "Thuwis",
                Room = "AT109 A",
                Birthday = new DateTime(1937, 4, 8),
                Doctor = new Doctor
                {
                    Name = "Massimo Destino",
                    PhoneNumber = "089 84 29 87"
                },
                Tags = new List<int> {1, 2, 3, 4}
            },
            new Resident
            {
                Id = new ObjectId("5a95677d8b9ed54db08d0ce8"),
                FirstName = "Martha",
                LastName = "Schroyen",
                Room = "AT109 A",
                Birthday = new DateTime(1929, 5, 26),
                Doctor = new Doctor
                {
                    Name = "Luc Euben",
                    PhoneNumber = "089 38 51 57"
                },
                Tags = new List<int> {5, 6}
            },
            new Resident
            {
                Id = new ObjectId("5a9568328b9ed54db08d0ce9"),
                FirstName = "Roland",
                LastName = "Mertens",
                Room = "AQ230 A",
                Birthday = new DateTime(1948, 9, 19),
                Doctor = new Doctor
                {
                    Name = "Peter Potargent",
                    PhoneNumber = "089 35 26 87"
                },
                Tags = new List<int> {7, 9, 8}
            },
            new Resident
            {
                Id = new ObjectId("5a9568838b9ed54db08d0cea"),
                FirstName = "Maria",
                LastName = "Creces",
                Room = "SA347 A",
                Birthday = new DateTime(1934, 1, 26),
                Doctor = new Doctor
                {
                    Name = "Willy Denier - Medebo",
                    PhoneNumber = "089 35 47 22"
                },
                Tags = new List<int> {10}
            },
            new Resident
            {
                Id = new ObjectId("5a967fc4c45be323bc42b5d8"),
                FirstName = "Ludovica",
                LastName = "Van Houten",
                Room = "AQ468 A",
                Birthday = new DateTime(1933, 1, 25),
                Doctor = new Doctor
                {
                    Name = "Marcel Mellebeek",
                    PhoneNumber = "089 65 74 85"
                }
            },
        };

        public static List<ReceiverModule> TestReceiverModules { get; } = new List<ReceiverModule>
        {
            new ReceiverModule
            {
                Id = new ObjectId("5a996594c081860934bcadef"),
                IsActive = true,
                Mac = "dd:dd:dd:dd:dd:dd",
                Position = new Point
                {
                    X = 0.200073229417303,
                    Y = 0.395857307249712,
                }
            },
            new ReceiverModule
            {
                Id = new ObjectId("5a996a5dab36bd0804a0f986"),
                IsActive = true,
                Mac = "dd:dd:dd:dd:dd:12",
                Position = new Point
                {
                    X = 0.4,
                    Y = 0.8,
                }
            },
            new ReceiverModule
            {
                Id = new ObjectId("5a996f25ddc3c03954d2586f"),
                IsActive = false,
                Mac = "ad:aa:aa:aa:aa:aa",
                Position = new Point
                {
                    X = 0.622053872053872,
                    Y = 0.392156862745098,
                }
            },
        };

        public static List<MediaData> TestMediaData { get; } = new List<MediaData>
        {
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data =Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Instance.Next(new byte[10])
            }
        };
    }
}