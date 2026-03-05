import { defineStore } from 'pinia'
import { ref } from 'vue'
import issuesApi from '@/api/issuesApi'
import type {
  CreateIssueRequest,
  UpdateIssueRequest,
  IssueDto,
  IssuesDto,
  AddCommentRequest,
} from '@/types/issues'
import { useUsersStore } from '@/stores/usersStore'

export const useIssuesStore = defineStore('issues', () => {
  const issues = ref<IssuesDto[]>([])

  const fetchIssues = async (projectId: string): Promise<void> => {
    issues.value = await issuesApi.getIssues(projectId)
  }

  const clearIssues = (): void => {
    issues.value = []
  }

  const deleteIssue = async (projectId: string, issueId: string): Promise<void> => {
    issues.value = issues.value.filter((p) => p.id !== issueId)

    await issuesApi.deleteIssue(projectId, issueId)
  }

  const getIssueIdByKey = (issueKey: string): string | undefined => {
    return issues.value.find((i) => i.key === issueKey)?.id
  }

  const getIssue = async (projectId: string, issueId: string): Promise<IssueDto> => {
    const issue = await issuesApi.getIssue(projectId, issueId)

    const existing = issues.value.find((i) => i.id === issue.id)!
    existing.title = issue.title
    existing.status = issue.status
    existing.priority = issue.priority
    existing.assignee = issue.assignee

    issue.comments.sort((a, b) => b.createdAt.localeCompare(a.createdAt))

    return issue
  }

  const createIssue = async (projectId: string, request: CreateIssueRequest): Promise<IssueDto> => {
    const issue = await issuesApi.createIssue(projectId, request)

    await fetchIssues(projectId)

    return issue
  }

  const updateIssue = async (
    projectId: string,
    issueId: string,
    request: UpdateIssueRequest,
  ): Promise<IssueDto> => {
    await issuesApi.updateIssue(projectId, issueId, request)

    return await getIssue(projectId, issueId)
  }

  const addWatcher = async (
    projectId: string,
    issue: IssueDto,
    watcherId: string,
  ): Promise<void> => {
    const usersStore = useUsersStore()

    await issuesApi.addWatcher(projectId, issue.id, watcherId)

    if (!usersStore.users.length) {
      await usersStore.fetchUsers()
    }

    const user = usersStore.users.find((u) => u.id === watcherId)
    issue.watchers.push(user!)
  }

  const removeWatcher = async (
    projectId: string,
    issue: IssueDto,
    watcherId: string,
  ): Promise<void> => {
    await issuesApi.removeWatcher(projectId, issue.id, watcherId)

    issue.watchers = issue.watchers.filter((w) => w.id !== watcherId)
  }

  const addComment = async (
    projectId: string,
    issueId: string,
    request: AddCommentRequest,
  ): Promise<IssueDto> => {
    await issuesApi.addComment(projectId, issueId, request)

    return await getIssue(projectId, issueId)
  }

  const uploadAttachment = async (
    projectId: string,
    issueId: string,
    file: FormData,
  ): Promise<IssueDto> => {
    await issuesApi.uploadAttachment(projectId, issueId, file)

    return await getIssue(projectId, issueId)
  }

  const removeAttachment = async (
    projectId: string,
    issue: IssueDto,
    attachmentId: string,
  ): Promise<void> => {
    await issuesApi.removeAttachment(projectId, issue.id, attachmentId)

    issue.attachments = issue.attachments.filter((a) => a.id !== attachmentId)
  }

  return {
    issues,
    fetchIssues,
    clearIssues,
    deleteIssue,
    getIssueIdByKey,
    getIssue,
    createIssue,
    updateIssue,
    addWatcher,
    removeWatcher,
    addComment,
    uploadAttachment,
    removeAttachment,
  }
})
