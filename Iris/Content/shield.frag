#version 120

#define SPCOL1 (vec3(116.,255.,255.)/255.)
#define SPCOL2 (vec3(55.,213.,255.)/255.)
#define PI 3.14159265359

uniform vec2 resolution = vec2(800.,600.);
uniform vec2 center;

/*
vec4 fakeAlpha(vec4 inp) {
    vec3 underColor = vec3(209.,50.,54.)/255.;
    float clampedAlpha = clamp(inp.a, 0., 1.);
    vec3 resultFullAlpha = mix(underColor, inp.xyz, clampedAlpha);
    return vec4(resultFullAlpha, 1.);
}
*/

void main()
{
	vec4 fragColor;
	vec2 fragCoord = gl_FragCoord;

    vec2 uv = 2.* (fragCoord.xy / iResolution.y - vec2(.8,.5));
    
    float r = length(uv);
    float a = atan(uv.y,uv.x); // to polar
    float s2 = 3.*a + 2.*PI*log(r);
    float c2 = smoothstep(-.01, .01, sin(s2-iGlobalTime*20.));
    
    vec3 col = mix(SPCOL1, SPCOL2, c2);
    fragColor = vec4(col,log(r*7.)*r);
    fragColor.a = clamp(fragColor.a, 0., 1.);
    fragColor.a -= 0.;
    
    //omit in sfml
    //fragColor = fakeAlpha(fragColor);
	gl_FragColor = fragColor;
}