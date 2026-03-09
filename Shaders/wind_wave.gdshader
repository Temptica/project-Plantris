shader_type spatial;
render_mode cull_disabled, depth_draw_opaque, unshaded;

uniform sampler2D grass_texture : filter_nearest, source_color;

// New Parameters
uniform float wind_strength : hint_range(0.0, 5.0) = 1.0;
uniform vec2 wind_direction = vec2(1.0, 1.0); // Use 1.0 or -1.0 to flip axes

void vertex(){
    NORMAL = vec3(0.0, 1.0, 0.0);
    
    // Calculate sway amounts
    float sway_x = sin(NODE_POSITION_WORLD.x + TIME * 1.25 + UV.y) * (1.0 - UV.y);
    float sway_z = cos(NODE_POSITION_WORLD.z + TIME * 0.45 + UV.y) * (1.0 - UV.y);
    
    // Apply direction and strength
    VERTEX.x += sway_x * 0.2 * wind_strength * wind_direction.x;
    VERTEX.z += sway_z * 0.15 * wind_strength * wind_direction.y;
}

void fragment(){
    vec4 tex = texture(grass_texture, UV);
    ALBEDO = tex.rgb;
    ALPHA = tex.a;
    ALPHA_SCISSOR_THRESHOLD = 0.5;
    
    // Add these lines to brighten it up in PBR:
    METALLIC = 0.0;
    ROUGHNESS = 1.0; 
    SPECULAR = 0.0;
}