NoiseBall2
----------

This is a small example of procedural mesh generation with a compute shader.

![gif](http://i.imgur.com/3G64iyw.gif)

This example uses [DrawMeshInstancedIndirect] to draw a procedurally generated
mesh. Although the main purpose of this method is not procedural modeling,
it's the only way to procedurally populate triangles and draw them with using
a surface shader.

[DrawMeshInstancedIndirect]: https://docs.unity3d.com/ScriptReference/Graphics.DrawMeshInstancedIndirect.html
