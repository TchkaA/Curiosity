shader_type canvas_item;

// Сила вспышки: 0.0 = спрайт обычный, 1.0 = полностью залит цветом
uniform float flash_amount : hint_range(0.0, 1.0) = 0.0;

// Цвет вспышки (по умолчанию белый). source_color нужен, чтобы Godot понял, что это цвет.
uniform vec4 flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
    // 1. Берём оригинальный цвет пикселя из текстуры
    vec4 original_color = texture(TEXTURE, UV);
    
    // 2. Смешиваем его с цветом вспышки. 
    // mix(a, b, t) возвращает a, когда t=0, и b, когда t=1.
    vec3 final_rgb = mix(original_color.rgb, flash_color.rgb, flash_amount);
    
    // 3. Собираем финальный цвет, сохраняя оригинальную прозрачность (альфу).
    // Это важно, чтобы прозрачные части спрайта не заливались цветом.
    COLOR = vec4(final_rgb, original_color.a);
}