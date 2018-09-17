﻿
public struct AltUnityObjectAction {

	public string Component;
	public string Method;
	// public Dictionary<string, string> parameters;
	public string Parameters;

	public AltUnityObjectAction(string componentName = "", string methodName = "", string parameters = null) {
		Component = componentName;
		Method = methodName;
		Parameters = parameters;
	    Assembly = null;
	}

    public AltUnityObjectAction(string component, string method, string parameters, string assembly)
    {
        Component = component;
        Method = method;
        Parameters = parameters;
        Assembly = assembly;
    }

    public string Assembly;
}
