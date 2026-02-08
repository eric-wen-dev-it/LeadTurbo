using LeadTurbo.Artemis;
using LeadTurbo.Artemis.Attributes;
using System.ComponentModel.DataAnnotations;
namespace GoogleMapQuery.Entities
{
	public partial class PlaceDetail : Entity
	{
		[Flags]
		public enum Flag
		{
		}
	}
}
