shader_type spatial;
render_mode unshaded;

uniform vec2 tiling_factor = vec2(5.0, 5.0);

uniform sampler2D rayTexture;

vec2 rotateUV(float time) {
	
	vec2 textureOffset;
	
	textureOffset.x = time * 0.5;
	textureOffset.y = time * 1.2;
	
	return textureOffset;
}

void fragment() {
	
	
	vec2 textureOffset = rotateUV(TIME);
	
	vec2 tiled_uvs = UV * tiling_factor;

    ALBEDO = texture(rayTexture, tiled_uvs + textureOffset).xyz;
	
}

