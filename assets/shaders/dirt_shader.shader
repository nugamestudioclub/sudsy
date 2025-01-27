shader_type canvas_item;
uniform sampler2D image;


void fragment(){
	COLOR = texture(image, SCREEN_UV);
}