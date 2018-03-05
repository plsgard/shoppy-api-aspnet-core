﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Shoppy.Core.Auditing;

namespace Shoppy.Core.Users
{
    public class User : IdentityUser<Guid>, IFullAudited
    {
        public const int MaxFirstNameLength = 100;
        public const int MaxLastNameLength = 100;
        public const int MaxPictureUrlLength = 300;
        public const int MaxLoginLength = 300;
        public const int MinPasswordLength = 6;
        public const int MaxEmailLength = 100;
        public const int MaxUserNameLength = 100;

        [Required, StringLength(MaxFirstNameLength)]
        public string FirstName { get; set; }

        [Required, StringLength(MaxLastNameLength)]
        public string LastName { get; set; }

        [Required, StringLength(MaxPictureUrlLength), Url, DataType(DataType.Url)]
        public string PictureUrl { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }

        public User(string userName) : this()
        {
            UserName = userName;
        }

        public DateTimeOffset CreationTime { get; set; }
        public Guid? CreationUserId { get; set; }
        public DateTimeOffset? ModificationTime { get; set; }
        public Guid? ModificationUserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletionTime { get; set; }
        public Guid? DeletionUserId { get; set; }
    }
}
