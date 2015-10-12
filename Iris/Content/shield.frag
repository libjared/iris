#version 120

#define SPCOL1 (vec3(116.,255.,255.)/255.)
#define SPCOL2 (vec3(55.,213.,255.)/255.)
#define PI 3.14159265359

uniform vec2 resolution = vec2(800.,600.);
uniform float seconds;
uniform vec2 center;

void main()
{
	vec4 fragColor;
	vec2 fragCoord = gl_FragCoord.xy;
	vec2 iMouse = center;
	float iGlobalTime = seconds;
	vec2 iResolution = resolution;

    vec2 uv = 2.* (fragCoord.xy - iMouse.xy) / iResolution.y;

    float r = length(uv);
    float a = atan(uv.y,uv.x); // to polar
    float s2 = 3.*a + 2.*PI*log(r);
    float c2 = smoothstep(-1., 1., sin(s2-iGlobalTime*20.));
    
    vec3 col = mix(SPCOL1, SPCOL2, c2);
    fragColor = vec4(col,.5);

	gl_FragColor = fragColor;
}