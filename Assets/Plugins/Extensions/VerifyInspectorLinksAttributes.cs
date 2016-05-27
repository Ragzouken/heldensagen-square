using System;

/// <summary>
/// See companion script VerifyInspectorLinks.cs
/// </summary>

[AttributeUsage(AttributeTargets.Field)]
public class Optional : Attribute { }

[AttributeUsage(AttributeTargets.Field)]
public class SceneOnly : Attribute { }
