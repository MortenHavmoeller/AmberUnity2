using System.Runtime.Serialization;
using UnityEngine;

namespace ProjectAmber.SavingData.DataSurrogates
{
	public class Quaternion_SerializationSurrogate : ISerializationSurrogate
	{
		// serialize (commit)
		public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
		{
			Quaternion q = (Quaternion)obj;
			info.AddValue("x", q.x);
			info.AddValue("y", q.y);
			info.AddValue("z", q.z);
			info.AddValue("w", q.w);
		}

		// deserialize (retrieve)
		public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			Quaternion q = (Quaternion)obj;
			q.x = (float)info.GetValue("x", typeof(float));
			q.y = (float)info.GetValue("y", typeof(float));
			q.z = (float)info.GetValue("z", typeof(float));
			q.w = (float)info.GetValue("w", typeof(float));
			obj = q;
			return obj;
		}
	}
}
