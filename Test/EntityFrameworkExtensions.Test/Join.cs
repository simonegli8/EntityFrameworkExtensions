using System.Linq;
using System.Diagnostics;

namespace Test;

class Person
{
	public string Name;
	public string Address;
	public string Pet;
}

class Pet
{
	public string Name;
	public string Type;
}

public class TestJoin
{
	public void Test()
	{
		var persons = new Person[] {
			new Person() { Name = "Simon", Address = "Eggmattweg", Pet = "Cat" },
			new Person() { Name = "Maria", Address = "Parque", Pet = "Dog" }
		}
		.AsQueryable();

		var pets = new Pet[] {
			new Pet() { Name = "Murli", Type = "Cat" },
			new Pet() { Name = "Kugar", Type = "Dog" }
		}
		.AsQueryable();

		var query = persons
			.Join(pets, person => person.Pet, pet => pet.Type, (person, pet) => new Person()
			{
				Name = person.Name,
				Pet = pet.Type,
				Address = person.Address
			})
			.Where(person => person.Address == "Parque");

	}
}