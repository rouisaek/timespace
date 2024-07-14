<template>
    <!-- Begin form field -->
    <div class="field w-100 mt-6">
        <div class="p-float-label w-100">
            <Textarea :id="id" v-bind="componentAttributes" v-model="v$.modelValue.$model" :class="componentClasses">
                                            <template v-for="(_, slot) of $slots" v-slot:[slot]="scope">
                                                <slot :name="slot" v-bind="scope"/>
                                            </template>
                                        </Textarea>
            <label :for="id" :class="{ 'p-error': v$.modelValue.$invalid && showError }">{{ props.label }}{{ required ? '*'
                : '' }}</label>
        </div>
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
import { computed, reactive, useAttrs } from "vue";
import { getRules, type TextareaProps } from "../FormInputTypes";
import Textarea from "primevue/textarea";

const props = defineProps<TextareaProps>();

const modelValue = defineModel();
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const rules: any = getRules(props, false, props.label);

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue }));

// eslint-disable-next-line vue/no-setup-props-destructure

const componentClasses = reactive<any>({
    "p-invalid": computed(() => v$.value.modelValue.$invalid && props.showError),
    "w-100": true,
});

const inputProps = { ...props };
if (props.disabled) inputProps.disabled = props.disabled;
if (props.readonly) inputProps.readonly = props.readonly;
inputProps.rows = props?.rows ?? 3;

const componentAttributes = reactive<any>({ ...inputProps });

v$.value.$touch();
</script>