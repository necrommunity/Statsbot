using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statsbot
{

    public class Category
    {
        public bool Seeded { get; set; } = false;
        public Character Char { get; set; }
        public RunType Type { get; set; } = RunType.Score;
        public Mode Mode { get; set; } = Mode.Standard;
        public Product Product { get; set; } = Product.Amplified;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Category compare = obj as Category;
            if (Product == compare.Product && Seeded == compare.Seeded && Char == compare.Char && Type == compare.Type && Mode == compare.Mode)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (Char.GetHashCode() + Type.GetHashCode() + Seeded.GetHashCode() + Product.GetHashCode());
        }

    }


}
