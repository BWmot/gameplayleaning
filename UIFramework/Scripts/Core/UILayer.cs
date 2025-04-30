using Godot;
using System.Collections.Generic;

public partial class UILayer : Node
{
    private Dictionary<string, Node> _uiElements = new Dictionary<string, Node>();

    public void AddUIElement(string key, Node uiElement)
    {
        if (!_uiElements.ContainsKey(key))
        {
            _uiElements[key] = uiElement;
            AddChild(uiElement);
        }
    }

    public void RemoveUIElement(string key)
    {
        if (_uiElements.ContainsKey(key))
        {
            Node uiElement = _uiElements[key];
            RemoveChild(uiElement);
            _uiElements.Remove(key);
        }
    }

    public void ShowUIElement(string key)
    {
        if (_uiElements.ContainsKey(key) && _uiElements[key] is CanvasItem canvasItem)
        {
            canvasItem.Visible = true;
        }
    }

    public void HideUIElement(string key)
    {
        if (_uiElements.ContainsKey(key) && _uiElements[key] is CanvasItem canvasItem)
        {
            canvasItem.Visible = false;
        }
    }

    public void ClearAllUIElements()
    {
        foreach (var key in new List<string>(_uiElements.Keys))
        {
            RemoveUIElement(key);
        }
    }
}