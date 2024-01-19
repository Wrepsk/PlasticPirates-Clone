# Worldbuilding

In this folder the game world is being build!  
This Readme file covers information about the worldbuilding process.

## TODO

- [x] make all .fbx models to prefabs with respective materials
    - [x] fix sizes of those models/prefabs (conversion from blender to unity)
    - [x] apply materials to all LODs (fixed when using prefabs?)
    - [x] rename models/material/textures appropriately
    - [x] add mesh collider to LOD0 meshes
        - [ ] fix the mesh collider: for some assets in game it is not working... too big? too many triangles?
- [ ] make cartoony style textures
    - [ ] use appropriate materials/shaders in scene (south sand, north snow, ...)
- [ ] fix UVs
- [ ] clean unseen faces (create new models in blender?)
- [ ] NavMeshSurface generation (main boat and cargo ship)

## Asset Workflow

The workflow from a blender generated mesh to a functioning prefab is as follows

---

### Blender
1. gather references
1. model the mesh
    - basic modeling
    - geometry nodes generation
1. uv unwrap the mesh
1. coloring (shader/texture)
    - shader has to use textures (simple texture setup with basecolor/diffuse, normal, smoothness/roughness)
        - search and import textures (simple using add-on)
        - generate own texture by applying different shaders/textures
            - to
                - all of the mesh 
                - specific selection of mesh
                    - capture an attribute in geometry nodes, e.g. normal angle same direction as world up-axis
                    - vertex painting and using the respective "Color Attribute" (see [source](https://www.youtube.com/watch?v=0lj643VmTsg&ab_channel=CGGeek))
                    - using "Vertex Groups" with manual vertex selection
            - bake to texture! (see [source](https://www.youtube.com/watch?v=yloupOUjMOA&ab_channel=RigorMortisTortoise))
                - basecolor, normal and roughness separately (combined possible?)
                - render/bake to a new image
                - save the new image
            - (!) redo the shader in blender such that its a simple texture setup
    - shader (later in unity)
1. generate LODs of the mesh (simple using add-on "LODs generator")
1. export (all those LODs) as .fbx file

---

### Unity
1. import .fbx model (consists of mesh and material) and textures
1. scale model via "Convert Units" option of the .fbx model
1. make the model prefabs
1. place models
    - manually
    - using PolyBrush

---

 iterate to delete unnecessary faces from the meshes (check UVs!)

---

## Credits

Credits for some assets go to the following repositories

### [Advanced-Triplanar-Shader](https://github.com/GameDevBox/Advanced-Triplanar-Shader)

From here we took the *triplanar shader* to create the texture covering ontop (y-direction) of the prefabs

### [BoatAttack](https://github.com/Unity-Technologies/BoatAttack)

From here we took the inspiration
