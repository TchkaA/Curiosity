shader_type canvas_item;

uniform vec4 outline_color : source_color = vec4(1.0);
uniform float outline_size = 1.0;

void fragment()
{
    vec4 color = texture(TEXTURE, UV);

    float alpha = color.a;

    vec2 size = TEXTURE_PIXEL_SIZE * outline_size;

    float outline = 0.0;

    outline += texture(TEXTURE, UV + vec2(size.x, 0.0)).a;
    outline += texture(TEXTURE, UV - vec2(size.x, 0.0)).a;
    outline += texture(TEXTURE, UV + vec2(0.0, size.y)).a;
    outline += texture(TEXTURE, UV - vec2(0.0, size.y)).a;

    if (alpha <= 0.0 && outline > 0.0)
    {
        COLOR = outline_color;
    }
    else
    {
        COLOR = color;
    }
}