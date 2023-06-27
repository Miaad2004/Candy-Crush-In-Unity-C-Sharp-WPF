using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Models
{
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; protected set; }
        public int SequentialIndex { get; set; }

        public Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}
