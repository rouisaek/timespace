<template>
    <!-- Begin form field -->
    <div class="field w-100 mt-6">
        <label :for="id" :class="{ 'p-error': v$.modelValue.$invalid && showError }">{{ props.label }}{{ required ? '*'
            : '' }}</label>
        <FileUpload name="files[]" :url="props.uploadUrl" @beforeSend="uploadFile($event)" auto withCredentials
            @upload="afterUpload($event)" @error="handleUploadError($event)" :class="componentClasses"
            :showUploadButton="false" :show-cancel-button="false" ref="inputRef"
            @remove="removeFile($event)" @removeUploadedFile="removeFile($event)" v-bind="componentAttributes">
            <template #empty>
                <p>{{ $t('fileInput.dragAndDropMessage') }}</p>
            </template>
        </FileUpload>
        <small :id="id + '-help'" v-if="helpText">{{ helpText }}<br></small>
        <span v-if="v$.modelValue.$invalid && showTextErrors">
            <span :id="id + '-error'" v-for="(error, index) of v$.modelValue.$errors" :key="index">
                <small class="p-error">{{ error.$message }}</small>
            </span>
        </span>
    </div>
    <!-- End form Field -->
</template>

<script lang="ts">
export default { inheritAttrs: false };
</script>

<script setup lang="ts">
import { useVuelidate } from "@vuelidate/core";
import { uniqueId } from "lodash-es";
import { reactive, ref, useAttrs } from "vue";
import { getRules, type InputFileProps, type UploadedFile } from "../FormInputTypes";
import FileUpload, { type FileUploadBeforeSendEvent, type FileUploadErrorEvent, type FileUploadRemoveEvent, type FileUploadRemoveUploadedFile, type FileUploadState, type FileUploadUploadEvent } from "primevue/fileupload";
import { useI18n } from "vue-i18n";
import prettyBytes from "pretty-bytes";

const inputRef = ref<FileUploadState | null>(null);

const props = defineProps<InputFileProps>();

const localFiles = ref<UploadedFile[]>([]);

const modelValue = defineModel<UploadedFile[]>();
const uploading = defineModel<boolean>("uploading");
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const { t } = useI18n();

const rules: any = getRules(props, false, props.label);

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue }));

// eslint-disable-next-line vue/no-setup-props-destructure

const componentClasses = reactive<any>({
    "p-invalid": true,
    "w-100": true,
});

const componentAttributes = reactive<any>({ ...props, ...attrs });

function uploadFile(event: FileUploadBeforeSendEvent) {
    uploading.value = true;
    event.xhr.setRequestHeader("Accept", "application/json");
}

function afterUpload(e: FileUploadUploadEvent) {
    var json = JSON.parse(e.xhr.response);
    modelValue.value = [...(modelValue.value ?? []), ...json.files.map((f: any) => ({ name: f.filename, hash: f.sha1_hash, id: f.id }))];
    localFiles.value = [...(localFiles.value ?? []), ...json.files.map((f: any) => ({ name: f.filename, hash: f.sha1_hash, id: f.id }))];
    uploading.value = false;
}

function removeFile(event: FileUploadRemoveEvent | FileUploadRemoveUploadedFile) {
    const localFile = localFiles.value.find((lf) => lf.name === event.file.name);
    const filteredModelValue = modelValue.value?.filter((f) => f !== localFile);
    localFiles.value = localFiles.value.filter((f) => f.name !== event.file.name);
    modelValue.value = filteredModelValue;
    inputRef.value!.uploadedFileCount = inputRef.value!.uploadedFileCount - 1;
}

function handleUploadError(e: FileUploadErrorEvent) {
    if (inputRef.value !== null) {
        if (Array.isArray(e.files)) {
            e.files.forEach((file) => {
                inputRef.value!.files = inputRef.value!.files.filter((f) => f.name !== file.name);
            });
        }
    }
    console.log(e, inputRef.value);
}

v$.value.$touch();
</script>
