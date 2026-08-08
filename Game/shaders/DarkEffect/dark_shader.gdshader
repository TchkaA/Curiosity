shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;

uniform float exposure : hint_range(0.0, 2.0) = 0.7;

void fragment()
{
    vec4 color = texture(screen_texture, SCREEN_UV);

    color.rgb *= exposure;

    COLOR = color;
}