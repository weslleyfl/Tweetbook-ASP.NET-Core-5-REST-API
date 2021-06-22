﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tweetbook.Domain
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(100, ErrorMessage = "Tamanho maximo permitido é até 100 caracteres.")]
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        public virtual List<PostTag> Tags { get; set; }
    }
}
