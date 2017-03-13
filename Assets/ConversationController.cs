using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine.UI;


public class ConversationController : MonoBehaviour {

	public class ConversationNode
	{
		public string dialog;
		public string[] optionStrings;
		public int[] optionRedirects;
		public string emotion;

		public ConversationNode(){
			dialog = "";
			emotion = "default";
			optionStrings = new string[4];
			for (int i = 0; i<4; i++){
				optionStrings[i] = "";
			}
			optionRedirects = new int[4];
			for (int i = 0; i<4; i++){
				optionRedirects[i] = -2;
			}
		}
	}

	public class Conversation
	{

		private Dictionary<int, ConversationNode> nodes;
		public string characterName = "";
		public string characterImageFilePath = "";


		public Conversation(string filename) {
			nodes = new Dictionary<int, ConversationNode>();
			TextAsset xmlFile = (TextAsset)Resources.Load(filename);
			if (xmlFile != null) {
				Debug.Log("Found conversation file at "+filename);
				XmlTextReader reader = new XmlTextReader(new StringReader(xmlFile.text));
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlFile.text);
				foreach(XmlNode node in doc.DocumentElement.ChildNodes){
					if(node.Name == "character_full_name") {
						characterName = node.InnerText;
					}

					if(node.Name == "character_reference_name") {
						string name = node.InnerText;
						characterImageFilePath = "Characters/"+name+"/"+name+"-";
					}

					if(node.Name == "node") {
						ConversationNode s = new ConversationNode();
						int id = -1;
						foreach(XmlNode convonode in node.ChildNodes){
							if(convonode.Name == "id") {
								id = int.Parse(convonode.InnerText);
							}

							if(convonode.Name == "dialog") {
								s.dialog = convonode.InnerText;
							}
							if(convonode.Name == "emotion") {
								s.emotion = convonode.InnerText;
								if (s.emotion == ""){
									s.emotion = "default";
								}
							}
							if(convonode.Name == "option1") {
								s.optionStrings[0] = convonode.InnerText;
							}
							if(convonode.Name == "option2") {
								s.optionStrings[1] = convonode.InnerText;
							}
							if(convonode.Name == "option3") {
								s.optionStrings[2] = convonode.InnerText;
							}
							if(convonode.Name == "option4") {
								s.optionStrings[3] = convonode.InnerText;
							}
							if(convonode.Name == "redirect1") {
								string raw = convonode.InnerText;
								if(raw != ""){
									s.optionRedirects[0] = int.Parse(raw);
								}
							}
							if(convonode.Name == "redirect2") {
								string raw = convonode.InnerText;
								if(raw != ""){
									s.optionRedirects[1] = int.Parse(raw);
								}
							}
							if(convonode.Name == "redirect3") {
								string raw = convonode.InnerText;
								if(raw != ""){
									s.optionRedirects[2] = int.Parse(raw);
								}
							}
							if(convonode.Name == "redirect4") {
								string raw = convonode.InnerText;
								if(raw != ""){
									s.optionRedirects[3] = int.Parse(raw);
								}
							}
						}
						Debug.Log("Read conversation node ID "+id);
						nodes.Add(id, s);
					}
				}
			}else{
				Debug.Log("Unable to find conversation file "+filename);
			}
			Debug.Log("Read "+nodes.Count+" conversation nodes from XML");
		}

		public ConversationNode getNode(int node) {
			if (nodes.ContainsKey(node)) {
				return nodes [node];
			} else {
				Debug.Log("Unable to find node "+node);
				return null;
			}
		}
	}

	Conversation c;
	public GameObject CharacterName;
	public GameObject CharacterImage;
	public string characterImageFilePath;
	public GameObject Dialog;
	public GameObject[] optionButtons;

	int CurrentNode = 0;

	// Use this for initialization
	void Start () {
		c = new Conversation ("Characters/Test/Conversations/Testconvo1");
		CharacterName.GetComponent<Text>().text = c.characterName;
		characterImageFilePath = c.characterImageFilePath;
		SetUpConversationNode (CurrentNode);
	}

	void SetUpConversationNode(int node){
		CurrentNode = node;
		if (node != -1) {
			ConversationNode currentNode = c.getNode (node);
			if (currentNode != null) {
				Dialog.GetComponent<Text> ().text = currentNode.dialog;
				Sprite Image = Resources.Load<Sprite> (characterImageFilePath + currentNode.emotion);
				if (Image != null) {
					CharacterImage.GetComponent<Image> ().sprite = Image;
				} else {
					Debug.Log ("Unable to load character image at " + characterImageFilePath);
				}
				for (int option = 0; option < 4; option++) {
					optionButtons [option].GetComponent<Button> ().interactable = (currentNode.optionRedirects [option] != -2);
					optionButtons [option].transform.FindChild ("Text").gameObject.GetComponent<Text> ().text = currentNode.optionStrings [option];
				}
			}
		} else {
			Dialog.GetComponent<Text> ().text = "[END OF CONVERSATION]";
			for (int option = 0; option < 4; option++) {
				optionButtons [option].GetComponent<Button> ().interactable = false;
				optionButtons [option].transform.FindChild ("Text").gameObject.GetComponent<Text> ().text = "";
				Sprite Image = Resources.Load<Sprite> (characterImageFilePath + "default");
				if (Image != null) {
					CharacterImage.GetComponent<Image> ().sprite = Image;
				} else {
					Debug.Log ("Unable to load character image at " + characterImageFilePath);
				}
			}
		}
	}

	public void GoToDialogOption(int option) {
		ConversationNode currentNode = c.getNode (CurrentNode);
		if (currentNode != null) {
			SetUpConversationNode (currentNode.optionRedirects [option]);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}


}
