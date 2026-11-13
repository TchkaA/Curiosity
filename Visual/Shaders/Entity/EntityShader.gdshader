shader_type canvas_item;

// Вспышка
uniform float flash_amount : hint_range(0.0, 1.0) = 0.0;
uniform vec4 flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

// Базовая обводка
uniform bool outline_enabled = true;
uniform float outline_width : hint_range(0.0, 16.0) = 2.0;
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float alpha_threshold : hint_range(0.0, 1.0) = 0.1;

// Пульсация
uniform float outline_pulse_amount : hint_range(0.0, 8.0) = 0.0;
uniform float outline_pulse_speed : hint_range(0.0, 20.0) = 3.0;

// Градиент
uniform bool outline_gradient_enabled = false;
uniform vec4 outline_color2 : source_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform bool outline_gradient_vertical = true;

// Глитч
uniform bool glitch_enabled = false;
uniform float glitch_intensity : hint_range(0.0, 100.0) = 5.0;
uniform float glitch_speed : hint_range(0.0, 30.0) = 10.0;
uniform float glitch_rows : hint_range(1.0, 200.0) = 30.0;

// Тонирование кожи
uniform vec4 skin_tint : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float skin_brightness : hint_range(0.0, 10.0) = 1.0;
uniform float eye_threshold : hint_range(0.0, 1.0) = 0.5;

const int SAMPLES = 16;

bool has_opaque_neighbor(sampler2D tex, vec2 uv, vec2 texel_size, float radius) {
    for (int i = 0; i < SAMPLES; i++) {
        float angle = (float(i) / float(SAMPLES)) * 6.28318530718;
        vec2 dir = vec2(cos(angle), sin(angle));
        vec2 sample_uv = uv + dir * texel_size * radius;
        if (texture(tex, sample_uv).a > alpha_threshold) {
            return true;
        }
    }
    return false;
}

float hash11(float x) {
    return fract(sin(x * 12.9898) * 43758.5453);
}

vec2 apply_glitch(vec2 uv, vec2 texel) {
    if (!glitch_enabled || glitch_intensity <= 0.0) return uv;

    float row = floor(uv.y * glitch_rows);
    float t = floor(TIME * glitch_speed);
    float r = hash11(row + t);

    float shift = 0.0;
    if (r > 0.7) {
        float r2 = hash11(row * 2.0 + t * 1.37);
        shift = (r2 - 0.5) * 2.0;
    }

    uv.x += shift * glitch_intensity * texel.x;
    return uv;
}

vec4 get_outline_color(vec2 uv) {
    if (!outline_gradient_enabled) return outline_color;

    float t = outline_gradient_vertical ? uv.y : uv.x;

    // Жёсткая граница: до порога — первый цвет, после — второй.
    // step(edge, x) возвращает 0 если x < edge, и 1 если x >= edge.
    // 0.5 — порог ровно посередине спрайта. Меняй, если нужно сместить границу.
    return mix(outline_color, outline_color2, step(0.5, t));
}

void fragment() {
    vec2 uv = apply_glitch(UV, TEXTURE_PIXEL_SIZE);
    vec4 original_color = texture(TEXTURE, uv);

    float lum = dot(original_color.rgb, vec3(0.299, 0.587, 0.114));
    float skin_mask = 1.0 - smoothstep(eye_threshold - 0.1, eye_threshold + 0.1, lum);
    vec3 tinted = original_color.rgb * skin_tint.rgb * skin_brightness;
    vec3 skinned_rgb = mix(original_color.rgb, tinted, skin_mask);

    float pulse_width = outline_width;
    if (outline_pulse_amount > 0.0) {
        float p = sin(TIME * outline_pulse_speed) * 0.5 + 0.5;
        pulse_width += p * outline_pulse_amount;
    }

    vec4 outline_contribution = vec4(0.0);

    if (outline_enabled && pulse_width > 0.0) {
        vec2 texel = TEXTURE_PIXEL_SIZE;

        if (original_color.a <= alpha_threshold) {
            if (has_opaque_neighbor(TEXTURE, uv, texel, pulse_width)) {
                outline_contribution = get_outline_color(UV);
            }
        }
    }

    vec3 final_rgb = mix(skinned_rgb, flash_color.rgb, flash_amount);
    vec4 final_color = vec4(final_rgb, original_color.a);

    COLOR = mix(outline_contribution, final_color, original_color.a);
}