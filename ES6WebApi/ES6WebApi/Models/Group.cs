using System;
namespace ES6WebApi.Models
{
	public class Group
	{

        public int Id { get; set; }
        public string Name { get; set; }

		public Group(string name)
		{
			Name = name;
		}

	}
}

