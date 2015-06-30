#version 120

uniform sampler2D sampler;
uniform float ouch = 0; //1 is ouch

void main() {
	//tex alpha, early quit
	vec4 pixel = texture2D(sampler, gl_TexCoord[0].xy);
	if (pixel.a == 0) {
		discard;
	}
	if (ouch < 0.5) {
		gl_FragColor = pixel;
		return;
	} else {
		gl_FragColor = vec4(1,1,1,1);
	}
}
