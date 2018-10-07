{{- define "fqdn-image" -}}
{{- if .Values.inf.registry -}}
{{- printf "%s/%s" .Values.inf.registry.server .Values.image.repository -}}
{{- else -}}
{{- .Values.image.repository -}}
{{- end -}}
{{- end -}}