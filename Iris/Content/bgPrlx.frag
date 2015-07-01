#version 120

uniform vec2 resolution = vec2(800.,600.);
uniform float offsetY = 0;

void main() {
	vec2 fc = gl_FragCoord.xy - vec2(0, offsetY * 2);
	vec2 uv = fc / resolution.xy;
	float r = (142./255.)-(53./255.*2.)*uv.y;
	float g = (202./255.)-(12./255.*2.)*uv.y;
	float b = 1.;
	vec3 sky = vec3(r,g,b);
	gl_FragColor = vec4(sky,1.);
}
