using ProtoBuf;
using LeadTurbo.Artemis;
using LeadTurbo.Artemis.Attributes;
using System.ComponentModel.DataAnnotations;
namespace GoogleMapQuery.Entities
{
	[ProtoContract]
	public partial class KeywordTranslation : Entity
	{
		protected override object Create()
		{
			return new KeywordTranslation();
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

		string language=default(string);
		[StringLength(255)]
		[ProtoMember(103)]
		public string Language
		{
			get => language;
			set => SetValidatedProperty(ref language, value);
		}

		string originalKeyword=default(string);
		[StringLength(255)]
		[ProtoMember(104)]
		public string OriginalKeyword
		{
			get => originalKeyword;
			set => SetValidatedProperty(ref originalKeyword, value);
		}

		string targetKeyword=default(string);
		[StringLength(255)]
		[ProtoMember(105)]
		public string TargetKeyword
		{
			get => targetKeyword;
			set => SetValidatedProperty(ref targetKeyword, value);
		}

		string editor=default(string);
		[StringLength(50)]
		[ProtoMember(106)]
		public string Editor
		{
			get => editor;
			set => SetValidatedProperty(ref editor, value);
		}

		DateTime editorTime=default(DateTime);
		[ProtoMember(107)]
		public DateTime EditorTime
		{
			get => editorTime;
			set => SetValidatedProperty(ref editorTime, value);
		}
	}
}
