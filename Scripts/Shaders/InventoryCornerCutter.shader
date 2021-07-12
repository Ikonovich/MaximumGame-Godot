// Credit to pouleyKetchoupp at www.nekomatata.com for the example heavily relied upon to make this.

shader_type canvas_item;
render_mode blend_mix, unshaded;



uniform float radius = 0.278;

uniform vec4 rgba = vec4(1.0, 1.0, 1.0, 1.0);

uniform vec2 size;


void fragment() {
	

    vec2 ratio = vec2(max(1.0, size.x / size.y), max(1.0, size.y / size.x));  // Gets the ratio of the two sizes of the object.

    vec4 texture_item = texture(TEXTURE, UV);
    COLOR = rgba * texture_item;


    float half_radius = radius / 2.0;
    vec2 max_distance = half_radius / ratio;
    vec2 position = clamp(UV, max_distance, 1.0 - max_distance);
    float distance_actual = distance(UV * ratio, position * ratio);
    COLOR.a *= step(distance_actual, half_radius + 0.000001);



}