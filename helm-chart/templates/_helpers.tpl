{{/*
Expand the name of the chart.
*/}}
{{- define "youtube-live-chat-to-discord.name" -}}
{{- default $.Chart.Name $.Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "youtube-live-chat-to-discord.fullname" -}}
{{- if $.Values.fullnameOverride }}
{{- $.Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default $.Chart.Name $.Values.nameOverride }}
{{- if contains $name $.Release.Name }}
{{- $.Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" $.Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "youtube-live-chat-to-discord$.Chart" -}}
{{- printf "%s-%s" $.Chart.Name $.Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "youtube-live-chat-to-discord.labels" -}}
helm.sh/chart: {{ include "youtube-live-chat-to-discord$.Chart" . }}
{{ include "youtube-live-chat-to-discord.selectorLabels" . }}
{{- if $.Chart.AppVersion }}
app.kubernetes.io/version: {{ $.Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ $.Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "youtube-live-chat-to-discord.selectorLabels" -}}
app.kubernetes.io/name: {{ include "youtube-live-chat-to-discord.name" . }}
app.kubernetes.io/instance: {{ $.Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "youtube-live-chat-to-discord.serviceAccountName" -}}
{{- if $.Values.serviceAccount.create }}
{{- default (include "youtube-live-chat-to-discord.fullname" .) $.Values.serviceAccount.name }}
{{- else }}
{{- default "default" $.Values.serviceAccount.name }}
{{- end }}
{{- end }}
