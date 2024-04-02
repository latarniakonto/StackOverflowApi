
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using AspNetCore.Identity.MongoDbCore.Models;

namespace StackOverflow.Infrastructure.Authorization;

[CollectionName("TagRole")]
public class TagRole : MongoIdentityRole<ObjectId>
{
    public TagRole() : base()
	{
	}

	public TagRole(string roleName) : base(roleName)
	{
	}
}
