
#pragma strict

@script ExecuteInEditMode

@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Mobile Bloom V2") 

public var intensity : float = 0.7f;
public var threshhold : float = 0.75f;
public var blurWidth : float = 1.0f;

public var extraBlurry : boolean = false;

// image effects materials for internal use

public var bloomMaterial : Material = null;

private var supported : boolean = false;

private var tempRtA : RenderTexture = null;
private var tempRtB : RenderTexture = null;

function Supported () : boolean {
	if(supported) return true;
	supported = (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && bloomMaterial.shader.isSupported);
	return supported;
}

function CreateBuffers () {			
	if (!tempRtA) {
		tempRtA = new RenderTexture (Screen.width / 4, Screen.height / 4, 0);
		tempRtA.hideFlags = HideFlags.DontSave;		
	}
	if (!tempRtB) {
		tempRtB = new RenderTexture (Screen.width / 4, Screen.height / 4, 0);	
		tempRtB.hideFlags = HideFlags.DontSave;
	}
}

function OnDisable () {
	if (tempRtA) {
		DestroyImmediate (tempRtA);
		tempRtA = null;
	}	
	if (tempRtB) {
		DestroyImmediate (tempRtB);
		tempRtB = null;
	}	
}

function EarlyOutIfNotSupported (source : RenderTexture, destination : RenderTexture) : boolean {
	if (!Supported ()) {
		enabled = false;
		Graphics.Blit (source, destination);	
		return true;
	}	
	return false;
}

function OnRenderImage (source : RenderTexture, destination : RenderTexture) {		
	 CreateBuffers ();
	if (EarlyOutIfNotSupported (source, destination))
		return;
	
	// prepare data
	
	bloomMaterial.SetVector ("_Parameter", Vector4 (0.0f,  0.0f, threshhold, intensity / (1.0f - threshhold)));	
	
	// ds & blur
	
	var oneOverW : float = 1.0f / (source.width * 1.0f);
	var oneOverH : float = 1.0f / (source.height * 1.0f);

	bloomMaterial.SetVector("_OffsetsA", Vector4(1.5f*oneOverW,1.5f*oneOverH,-1.5f*oneOverW,1.5f*oneOverH));	
	bloomMaterial.SetVector("_OffsetsB", Vector4(-1.5f*oneOverW,-1.5f*oneOverH,1.5f*oneOverW,-1.5f*oneOverH));	

	Graphics.Blit (source, tempRtB, bloomMaterial, 1);
	
	oneOverW *= 4.0f * blurWidth;
	oneOverH *= 4.0f * blurWidth;
	
	bloomMaterial.SetVector("_OffsetsA", Vector4(1.5f*oneOverW,0.0f,-1.5f*oneOverW,0.0f));	
	bloomMaterial.SetVector("_OffsetsB", Vector4(0.5f*oneOverW,0.0f,-0.5f*oneOverW,0.0f));	
	Graphics.Blit (tempRtB, tempRtA, bloomMaterial, 2);
	
	bloomMaterial.SetVector("_OffsetsA", Vector4(0.0f,1.5f*oneOverH,0.0f,-1.5f*oneOverH));	
	bloomMaterial.SetVector("_OffsetsB", Vector4(0.0f,0.5f*oneOverH,0.0f,-0.5f*oneOverH));	
	Graphics.Blit (tempRtA, tempRtB, bloomMaterial, 2);
	
	if(extraBlurry) {
		bloomMaterial.SetVector("_OffsetsA", Vector4(1.5f*oneOverW,0.0f,-1.5f*oneOverW,0.0f));	
		bloomMaterial.SetVector("_OffsetsB", Vector4(0.5f*oneOverW,0.0f,-0.5f*oneOverW,0.0f));	
		Graphics.Blit (tempRtB, tempRtA, bloomMaterial, 2);
		
		bloomMaterial.SetVector("_OffsetsA", Vector4(0.0f,1.5f*oneOverH,0.0f,-1.5f*oneOverH));	
		bloomMaterial.SetVector("_OffsetsB", Vector4(0.0f,0.5f*oneOverH,0.0f,-0.5f*oneOverH));	
		Graphics.Blit (tempRtA, tempRtB, bloomMaterial, 2);		
	}
	
	// bloomMaterial
	
	bloomMaterial.SetTexture ("_Bloom", tempRtB);
	Graphics.Blit (source, destination, bloomMaterial, 0);
}
