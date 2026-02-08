using ProtoBuf;
using LeadTurbo.Artemis;
using LeadTurbo.Artemis.Attributes;
using System.ComponentModel.DataAnnotations;
namespace GoogleMapQuery.Entities
{
	[ProtoContract]
	public partial class PlaceDetail : Entity
	{
		protected override object Create()
		{
			return new PlaceDetail();
		}



		int sequence=default(int);
		[Key]
		[SequenceProperty]
		[ProtoMember(102)]
		public int Sequence
		{
			get => sequence;
			set => SetValidatedProperty(ref sequence, value);
		}

		string placeID=default(string);
		[Key]
		[StringLength(255)]
		[ProtoMember(103)]
		public string PlaceID
		{
			get => placeID;
			set => SetValidatedProperty(ref placeID, value);
		}

		string name=default(string);
		[StringLength(255)]
		[ProtoMember(104)]
		public string Name
		{
			get => name;
			set => SetValidatedProperty(ref name, value);
		}

		string address=default(string);
		[StringLength(255)]
		[ProtoMember(105)]
		public string Address
		{
			get => address;
			set => SetValidatedProperty(ref address, value);
		}

		string types=default(string);
		[StringLength(255)]
		[ProtoMember(106)]
		public string Types
		{
			get => types;
			set => SetValidatedProperty(ref types, value);
		}

		string phoneNumber=default(string);
		[StringLength(255)]
		[ProtoMember(107)]
		public string PhoneNumber
		{
			get => phoneNumber;
			set => SetValidatedProperty(ref phoneNumber, value);
		}

		string internationalPhoneNumber=default(string);
		[StringLength(255)]
		[ProtoMember(108)]
		public string InternationalPhoneNumber
		{
			get => internationalPhoneNumber;
			set => SetValidatedProperty(ref internationalPhoneNumber, value);
		}

		string website=default(string);
		[StringLength(255)]
		[ProtoMember(109)]
		public string Website
		{
			get => website;
			set => SetValidatedProperty(ref website, value);
		}

		string images=default(string);
		[StringLength(4000)]
		[ProtoMember(110)]
		public string Images
		{
			get => images;
			set => SetValidatedProperty(ref images, value);
		}

		string editor=default(string);
		[StringLength(50)]
		[ProtoMember(111)]
		public string Editor
		{
			get => editor;
			set => SetValidatedProperty(ref editor, value);
		}

		DateTime editorTime=default(DateTime);
		[ProtoMember(112)]
		public DateTime EditorTime
		{
			get => editorTime;
			set => SetValidatedProperty(ref editorTime, value);
		}
	}
}
