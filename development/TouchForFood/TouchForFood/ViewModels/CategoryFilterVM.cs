using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
namespace TouchForFood.Models
{
    public class CategoryFilterVM
    {
        public menu m_menu { get; set; }
        public IList<category> m_category { get; set; }

        public CategoryFilterVM(menu menu, IList<category> category)
        {
            m_category = category;
            m_menu = menu;
        }

        public CategoryFilterVM()
        {
            m_category = new List<category>();
            m_menu = new menu();
        }

        public void addCategory(category category)
        {
            m_category.Add(category);
        }

        public category FirstOrDefault()
        {
            if (m_category.Count > 0)
            {
                return m_category.First();
            }
            m_category.Add(new category());
            return m_category.First();
        }
   }
}