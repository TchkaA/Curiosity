shader_type canvas_item;
render_mode unshaded;

// Текстура уже отрисованного экрана (Godot 4)
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

// --- Основные настройки ---
// Выбор размера матрицы Байера: 2x2, 4x4 или 8x8
uniform int bayer_size : hint_enum("2x2", "4x4", "8x8") = 2;

// Сила дизеринга. 0 = выключен, 1 = стандартный, >1 = агрессивнее
uniform float dither_strength : hint_range(0.0, 2.0) = 1.0;

// Количество уровней яркости после квантования.
// 2 = жёсткое ч/б (как раз для вашего контрастного стиля), 4–8 = больше градаций
uniform int color_levels : hint_range(2, 16) = 4;

// Размер «пикселя» дизеринга в экранных координатах.
// Увеличьте, если игра пиксельная и дизеринг должен быть крупнее.
uniform float dither_pixel_size : hint_range(1.0, 8.0) = 1.0;

// --- Тонкая настройка под тёмную сцену ---
// Смещение яркости до дизеринга. Помогает вытащить детали из тёмных участков.
uniform float brightness_offset : hint_range(-0.5, 0.5) = 0.0;

// Контраст. >1 усиливает разницу между тёмным и светлым.
uniform float contrast : hint_range(0.5, 2.0) = 1.0;

// Порог отсечения: пиксели темнее этого значения могут принудительно уходить в чёрный.
// Поставьте 0.0, чтобы отключить.
uniform float black_clip : hint_range(0.0, 0.3) = 0.0;

// --- Матрицы Байера ---
const int BAYER_2[4] = {0, 2, 3, 1};
const int BAYER_4[16] = {
    0, 8, 2, 10,
    12, 4, 14, 6,
    3, 11, 1, 9,
    15, 7, 13, 5
};
const int BAYER_8[64] = {
    0, 32, 8, 40, 2, 34, 10, 42,
    48, 16, 56, 24, 50, 18, 58, 26,
    12, 44, 4, 36, 14, 46, 6, 38,
    60, 28, 52, 20, 62, 30, 54, 22,
    3, 35, 11, 43, 1, 33, 9, 41,
    51, 19, 59, 27, 49, 17, 57, 25,
    15, 47, 7, 39, 13, 45, 5, 37,
    63, 31, 55, 23, 61, 29, 53, 21
};

float get_bayer_value(vec2 coord) {
    // Дискретизируем координаты под размер «пикселя» дизеринга
    vec2 pixel_coord = floor(coord / dither_pixel_size);
    int x = int(mod(pixel_coord.x, float(bayer_size)));
    int y = int(mod(pixel_coord.y, float(bayer_size)));

    if (bayer_size == 2) {
        return (float(BAYER_2[y * 2 + x]) + 0.5) / 4.0;
    } else if (bayer_size == 4) {
        return (float(BAYER_4[y * 4 + x]) + 0.5) / 16.0;
    } else {
        return (float(BAYER_8[y * 8 + x]) + 0.5) / 64.0;
    }
}

void fragment() {
    // 1. Читаем уже отрисованный экран
    vec3 color = texture(screen_texture, SCREEN_UV).rgb;

    // 2. Коррекция яркости/контраста (полезно для тёмных сцен)
    color = (color - 0.5) * contrast + 0.5 + brightness_offset;

    // 3. Опциональное отсечение слишком тёмных пикселей в чистый чёрный
    if (black_clip > 0.0) {
        float luma = dot(color, vec3(0.299, 0.587, 0.114));
        if (luma < black_clip) {
            color = vec3(0.0);
        }
    }

    // 4. Квантование (постеризация) с дизерингом
    float bayer = get_bayer_value(FRAGCOORD.xy);
    float levels_minus_one = float(color_levels - 1);

    // Ключевая формула: добавляем смещение Байера перед округлением.
    // Это заставляет соседние пиксели «голосовать» за разные уровни яркости.
    vec3 quantized;
    quantized.r = floor(color.r * levels_minus_one + bayer * dither_strength + 0.5) / levels_minus_one;
    quantized.g = floor(color.g * levels_minus_one + bayer * dither_strength + 0.5) / levels_minus_one;
    quantized.b = floor(color.b * levels_minus_one + bayer * dither_strength + 0.5) / levels_minus_one;

    // 5. Записываем результат
    COLOR.rgb = quantized;
    COLOR.a = 1.0;
}