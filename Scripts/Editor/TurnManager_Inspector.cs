using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TurnSystem))]
public class TurnManager_Inspector : Editor
{
    public VisualTreeAsset m_InspectorXML;

    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our inspector UI
        VisualElement myInspector = new VisualElement();

        // Load from default reference
        m_InspectorXML.CloneTree(myInspector);

        // Find the Foldout and Visual Elements
        var foldout = myInspector.Q<Foldout>("myFoldout");
        var visualElements = myInspector.Q<VisualElement>("myVisualElements");

        // Set initial state to false
        visualElements.style.display = DisplayStyle.None;

        // Register callback for Foldout state change
        foldout.RegisterValueChangedCallback(evt =>
        {
            visualElements.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        });
        
        // 버튼 가져오기
        Button myButton = myInspector.Q<Button>("StartButton");
        myButton.clicked += OnMyButtonClicked;

        // Return the finished inspector UI
        return myInspector;
    }

    public override bool UseDefaultMargins() => false;
    
    private void OnMyButtonClicked() {
        TurnSystem myComponent = (TurnSystem)target;
        myComponent.StartGame();
    }
}