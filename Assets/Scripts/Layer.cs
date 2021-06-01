using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Layer : MonoBehaviour
{
    //private
    const float LayerOffset = 200;

    static Vector3 Forward = new Vector3(1, 0, 0);

    //public
   public enum LayerType
	{
        Foreground = 1,
        Midground = 2,
        Background = 3,
        Inbetween = 0
	};

    public LayerType layerType;

    //How much offset from the default offset we want
    public float customLayerOffset = 0;

	private void Awake()
	{
        transform.position = Forward * (byte)layerType * LayerOffset;
	}

	// Start is called before tjhe first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
