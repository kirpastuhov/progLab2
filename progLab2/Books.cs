using System;
using System.Text;
using System.Runtime.Serialization;


namespace progLab2
{

    [DataContract]
    public class Books
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Annotation { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string ISBN { get; set; }

        //public DateTime PublicationDate;
        [DataMember]
        public string PublicationDate { get; set; }


        public string ToString(bool annotation = true)
        {

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Title: ").Append(Title).Append('\n');
            stringBuilder.Append("Author: ").Append(Author).Append('\n');
            stringBuilder.Append("ISBN: ").Append(ISBN).Append('\n');
            //stringBuilder.Append("PublicationDate: ").Append(PublicationDate.ToString("d-m-yyyy")).Append('\n');
            stringBuilder.Append("PublicationDate:").Append(PublicationDate).Append('\n');

            if (annotation)
            {
                stringBuilder.Append("Annotation: ").Append(Annotation).Append('\n');
            }

            return stringBuilder.ToString();
        }

        public override String ToString() => ToString(annotation: true);
    }
}
