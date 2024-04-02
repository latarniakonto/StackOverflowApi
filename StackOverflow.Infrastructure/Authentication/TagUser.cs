using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using AspNetCore.Identity.MongoDbCore.Models;

namespace StackOverflow.Infrastructure.Authentication;

[CollectionName("TagUser")]
public class TagUser : MongoIdentityUser<ObjectId>
{
	public TagUser() : base()
	{
	}

	public TagUser(string userName, string email) : base(userName, email)
	{
	}
}
