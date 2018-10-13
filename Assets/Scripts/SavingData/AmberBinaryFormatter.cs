using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using ProjectAmber.SavingData.DataSurrogates;

public class AmberBinaryFormatter
{
	private BinaryFormatter _binaryFormatter;
	
	public AmberBinaryFormatter()
	{
		// initialize
		_binaryFormatter = new BinaryFormatter();
		SurrogateSelector surrogateSelector = new SurrogateSelector();

		// add serialization surrogates
		surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3_SerializationSurrogate());
		surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new Quaternion_SerializationSurrogate());

		// apply to binary formatter
		_binaryFormatter.SurrogateSelector = surrogateSelector;
	}

	public void Serialize(System.IO.Stream serializationStream, object graph)
	{
		_binaryFormatter.Serialize(serializationStream, graph);
	}

	public object Deserialize(System.IO.Stream serializationStream)
	{
		return _binaryFormatter.Deserialize(serializationStream);
	}
}
