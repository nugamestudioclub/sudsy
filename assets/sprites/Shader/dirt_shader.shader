shader_type canvas_item;
render_mode skip_vertex_transform;
uniform sampler2D image;
uniform float opacity;
uniform mat4 global_transform;
varying vec2 world_position;

void vertex(){
    world_position = (global_transform * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment(){
	COLOR = texture(image,world_position.xy);
	//COLOR.a = 1f;
}