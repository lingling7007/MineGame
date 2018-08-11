using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

public class InmportSetting : AssetPostprocessor
{

	private void OnPostprocessModel (GameObject res)
	{
		//get object path  
		string path = assetImporter.assetPath;
		ModelImporter model = (ModelImporter)assetImporter; 
		if (path.Contains ("3DModel")) {
			model.animationType = ModelImporterAnimationType.Human;
		}
		if (path.Contains ("Animation")) {
			CreateAnimationController (model);
		}


	}

	private void CreateAnimationController (ModelImporter model)
	{
		string path = model.assetPath;



		Debug.LogError (model.defaultClipAnimations.Length+"  :  "+ path );

		return;


	
	}

	//	path = assetImporter.assetPath.Replace ("/" + res.name + ".FBX", string.Empty);
	//	int idx = path.LastIndexOf ('/') + 1;
	//	string acName = "Controller" + path.Substring (idx, path.Length - idx) + ".controller";
	//	AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController> (path + "/" + acName);
	//	//create animationController
	//	if (animatorController == null) {
	//		animatorController = AnimatorController.CreateAnimatorControllerAtPath (path + "/" + acName);
	//	}
	//
	//	Debug.LogError (model.defaultClipAnimations.Length + " : " + path + "\n" + acName + "\n" + path + "/" + acName);
	//	CreateAnimationController (assetImporter.assetPath,model);



}
